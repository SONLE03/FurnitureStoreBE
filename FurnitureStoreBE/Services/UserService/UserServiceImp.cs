using AutoMapper;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.MailRequest;
using FurnitureStoreBE.DTOs.Request.UserRequest;
using FurnitureStoreBE.DTOs.Response.UserResponse;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.Caching;
using FurnitureStoreBE.Services.FileUploadService;
using FurnitureStoreBE.Services.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace FurnitureStoreBE.Services.UserService
{
    public class UserServiceImp : IUserService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IMailService _mailService;
        private readonly IFileUploadService _fileUploadService;
        public UserServiceImp(ApplicationDBContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<User> userManager
            , IRedisCacheService redisCacheService, IMapper mapper, ITokenService tokenService, IMailService mailService, IFileUploadService fileUploadService)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _redisCacheService = redisCacheService;
            _mapper = mapper;
            _tokenService = tokenService;
            _mailService = mailService;
            _fileUploadService = fileUploadService;
        }
        public async Task<UserResponse> CreateUser(UserRequestCreate userRequest, string roleName)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                string email = userRequest.Email;
                string password = userRequest.Password;
                if (await _dbContext.Users.AnyAsync(u => u.Email == email))
                {
                    throw new ObjectAlreadyExistsException("User with this email already exists.");
                }

                var roleExists = await _roleManager.FindByNameAsync(roleName);
                if (roleExists == null)
                {
                    throw new ObjectNotFoundException($"{roleName} role does not exist.");
                }

                var newUser = new User
                {
                    Email = email,
                    UserName = email,
                    Role = roleName,
                    FullName = userRequest.FullName,
                    DateOfBirth = userRequest.DateOfBirth,
                    PhoneNumber = userRequest.PhoneNumber,
                    Cart = new Cart()
                };

                var createdUserResult = await _userManager.CreateAsync(newUser, password);
                if (!createdUserResult.Succeeded)
                {
                    throw new BusinessException("Failed to create account.");
                }

                var roleResult = await _userManager.AddToRoleAsync(newUser, roleName);
                if (!roleResult.Succeeded)
                {
                    throw new BusinessException("Failed to assign role to user.");
                }
                var claims = await _dbContext.RoleClaims.ToListAsync();
                var userClaims = claims
                    .Where(claim => userRequest.UserClaimsRequest.Contains(claim.Id))
                    .Select(claim => new Claim(claim.ClaimType, claim.ClaimValue))
                    .ToList();

                var claimsResult = await _userManager.AddClaimsAsync(newUser, userClaims);
                if (!claimsResult.Succeeded)
                {
                    throw new BusinessException("Failed to assign claims to user.");
                }

                // Send the account to user

                var sendAccount = new MailRequest
                {
                    ToEmail = email,
                    Subject = "Account",
                    Body = $"Your password: {password}"
                };
                await transaction.CommitAsync();
                return _mapper.Map<UserResponse>(newUser);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task UnbanUser(string userId)
        {
            var user = await _dbContext.Users
                   .Where(u => u.Id == userId)
                   .FirstAsync();
            if (user == null)
                throw new ObjectNotFoundException("User not found");
            user.IsLocked = false;
            await _dbContext.SaveChangesAsync();
            _tokenService.DeleteAllTokenByUserId(userId);
        }
        public async Task BanUser(string userId)
        {
            var user = await _dbContext.Users
                   .Where(u => u.Id == userId)
                   .FirstAsync();
            if (user == null)
                throw new ObjectNotFoundException("User not found");
            user.IsLocked = true;
            await _dbContext.SaveChangesAsync();
            _tokenService.DeleteAllTokenByUserId(userId);
        }
        public async Task DeleteUser(string userId)
        {
            var user = await _dbContext.Users
                    .Where(u => u.Id == userId)
                    .FirstAsync();
            if (user == null)
                throw new ObjectNotFoundException("User not found");
            user.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            _tokenService.DeleteAllTokenByUserId(userId);
        }

        public List<UserResponse> GetAllUsers(int role)
        {
            throw new NotImplementedException();
        }

        public async Task<ClaimsResult> GetClaimsByRole(int role)
        {
            var typeClaims = await _redisCacheService.GetData<List<TypeClaimsResponse>>(ERedisKey.typeClaims.ToString());
            if (typeClaims == null)
            {
                throw new Exception();
            }
            var roleClaims = await _redisCacheService.GetData<List<RoleClaimsResponse>>(ERedisKey.roleClaims.ToString());
            var filteredRoleClaims = roleClaims
                .Where(rc => rc.RoleId == role.ToString())
                .ToList();
            var claimsResult = new ClaimsResult
            {
                TypeClaims = typeClaims,
                RoleClaims = filteredRoleClaims
            };
            return claimsResult;
        }

        public async Task<UserResponse> UpdateUser(string userId, UserRequestUpdate userRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var user = await _dbContext.Users
                   .Where(u => u.Id == userId)
                   .FirstAsync();
                if (user == null)
                    throw new ObjectNotFoundException("User not found");
                user.DateOfBirth = userRequest.DateOfBirth;
                user.PhoneNumber = userRequest.PhoneNumber;
                user.FullName = userRequest.FullName;
                var createdUserResult = await _userManager.UpdateAsync(user);
                if (!createdUserResult.Succeeded)
                {
                    throw new BusinessException("Failed to update user.");
                }
                await transaction.CommitAsync();
                return _mapper.Map<UserResponse>(user);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateUserClaims(string userId, UserClaimsRequest userClaimsRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var user = await _dbContext.Users
                   .Where(u => u.Id == userId)
                   .FirstAsync();
                if (user == null)
                    throw new ObjectNotFoundException("User not found");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task ChangeAvatar(string userId, IFormFile avatarRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var user = await _dbContext.Users
                   .Where(u => u.Id == userId)
                   .FirstAsync();
                if (user == null)
                    throw new ObjectNotFoundException("User not found");
                var avatarUploadResult = await _fileUploadService.UploadFileAsync(avatarRequest, EUploadFileFolder.Avatar.ToString());
                var avatar = new Asset
                {
                    Name = avatarUploadResult.OriginalFilename,
                    URL = avatarUploadResult.Url.ToString(),
                    CloudinaryId = avatarUploadResult.PublicId,
                    FolderName = EUploadFileFolder.Avatar.ToString()
                };
                await _dbContext.Assets.AddAsync(avatar);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
