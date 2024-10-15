using AutoMapper;
using CloudinaryDotNet;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
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
using Microsoft.Extensions.Logging;
using System.Drawing.Printing;
using System.Linq;
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
        private readonly ILogger<UserServiceImp> _logger;
        public UserServiceImp(ApplicationDBContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<User> userManager
            , IRedisCacheService redisCacheService, IMapper mapper, ITokenService tokenService, IMailService mailService
            , IFileUploadService fileUploadService, ILogger<UserServiceImp> logger)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _redisCacheService = redisCacheService;
            _mapper = mapper;
            _tokenService = tokenService;
            _mailService = mailService;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }
        public async Task<PaginatedList<User>> GetAllUsers(string role, PageInfo pageInfo)
        {
            var usersQuery = _dbContext.Users.Where(u => u.Role == role);
            var count = await _dbContext.Users.CountAsync();
            return await Task.FromResult(PaginatedList<User>.ToPagedList(usersQuery, pageInfo.PageNumber, pageInfo.PageSize));
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
        public async Task<List<UserClaimsResponse>> GetUserClaims(string userId)
        {
            var user = await _dbContext.Users.AnyAsync(u => u.Id == userId);
            if (!user)
                throw new ObjectNotFoundException("User not found");

            var userClaims = await _dbContext.UserClaims
                                .Where(u => u.UserId == userId)
                                .Select(u => new UserClaimsResponse
                                {
                                    Id = u.Id,
                                    ClaimValue = u.ClaimValue
                                })
                                .ToListAsync();
            return userClaims;

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

        public async Task UpdateUserClaims(string userId, List<UserClaimsRequest> userClaimsRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var user = await _dbContext.Users.AnyAsync(u => u.Id == userId);
                if (!user)
                    throw new ObjectNotFoundException("User not found");
                var oldUserClaims = await _dbContext.UserClaims
                    .Where(u => u.UserId == userId)
                    .Select(u => new { u.Id, u.ClaimValue })
                    .ToListAsync();
                var newUserClaims = userClaimsRequest.Select(uc => new { uc.Id, uc.ClaimValue }).ToList();
                var claimsToRemove = oldUserClaims
                   .Where(oc => !newUserClaims.Any(nc => nc.ClaimValue == oc.ClaimValue))
                   .ToList();
                var claimsToAdd = newUserClaims
                    .Where(nc => !oldUserClaims.Any(oc => oc.ClaimValue == nc.ClaimValue))
                    .ToList();
                if (claimsToRemove.Any())
                {
                    var claimsToRemoveEntities = await _dbContext.UserClaims
                        .Where(uc => uc.UserId == userId && claimsToRemove.Select(c => c.Id).Contains(uc.Id))
                        .ToListAsync();

                    _dbContext.UserClaims.RemoveRange(claimsToRemoveEntities);
                }
                if (claimsToAdd.Any())
                {
                    var newClaims = await _dbContext.RoleClaims
                        .Where(rc => claimsToAdd.Select(c => c.Id).Contains(rc.Id))
                        .ToListAsync();
                    var userClaims = newClaims.Select(claim => new IdentityUserClaim<string>
                    {
                        UserId = userId,
                        ClaimType = claim.ClaimType,
                        ClaimValue = claim.ClaimValue,
                    });
                    await _dbContext.UserClaims.AddRangeAsync(userClaims);
                }
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
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
                    .FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new ObjectNotFoundException("User not found");
                }
                Asset avatar = new Asset();
                if (user.AssetId == null)
                {
                    avatar.User = user;
                }
                else
                {
                    avatar.Id = (Guid)user.AssetId;
                    await _fileUploadService.DestroyFileByAssetIdAsync(avatar.Id);
                }

                var avatarUploadResult = await _fileUploadService.UploadFileAsync(avatarRequest, EUploadFileFolder.Avatar.ToString());
                avatar.Name = avatarUploadResult.OriginalFilename;
                avatar.URL = avatarUploadResult.Url.ToString();
                avatar.CloudinaryId = avatarUploadResult.PublicId;
                avatar.FolderName = EUploadFileFolder.Avatar.ToString();
                if (user.AssetId == null)
                {
                    await _dbContext.Assets.AddAsync(avatar);
                }
                else
                {
                    _dbContext.Assets.Update(avatar);
                }
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<List<AddressResponse>> GetAddressesByUserId(string userId)
        {
            if (!await _dbContext.Users.AnyAsync(u => u.Id == userId))
            {
                throw new ObjectNotFoundException("User not found");
            }
            return _mapper.Map<List<AddressResponse>>(await _dbContext.Addresss.Where(u => u.UserId == userId && !u.IsDeleted).ToListAsync());
        }

        public async Task<AddressResponse> CreateUserAddress(string userId, AddressRequest addressRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (!await _dbContext.Users.AnyAsync(u => u.Id == userId))
                {
                    throw new ObjectNotFoundException("User not found");
                }
                var address = new Address
                {
                    Province = addressRequest.Province,
                    District = addressRequest.District,
                    Ward = addressRequest.Ward,
                    SpecificAddress = addressRequest.SpecificAddress,
                    PostalCode = addressRequest.PostalCode,
                    IsDefault = addressRequest.IsDefault,
                    UserId = userId,
                };
                if (addressRequest.IsDefault == true)
                {
                    var sql = "UPDATE \"Address\" SET \"IsDefault\" = @p0 WHERE \"UserId\" = @p1";
                    await _dbContext.Database.ExecuteSqlRawAsync(sql, false, userId);
                }
                await _dbContext.Addresss.AddAsync(address);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return _mapper.Map<AddressResponse>(address);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<AddressResponse> UpdateUserAddress(Guid addressId, AddressRequest addressRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var address = await _dbContext.Addresss.FirstOrDefaultAsync(u => u.Id == addressId);
                if (address == null)
                {
                    throw new ObjectNotFoundException("Address not found");
                }
                address.Province = addressRequest.Province;
                address.District = addressRequest.District;
                address.Ward = addressRequest.Ward;
                address.SpecificAddress = addressRequest.SpecificAddress;
                address.PostalCode = addressRequest.PostalCode;
                address.IsDefault = addressRequest.IsDefault;
                if (addressRequest.IsDefault == true)
                {
                    var sql = "UPDATE \"Address\" SET \"IsDefault\" = @p0 WHERE \"UserId\" = @p1";
                    await _dbContext.Database.ExecuteSqlRawAsync(sql, false, address.UserId);
                }
                _dbContext.Addresss.Update(address);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return _mapper.Map<AddressResponse>(address);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task DeleteUserAddress(Guid addressId)
        {
            try
            {
                if (!await _dbContext.Addresss.AnyAsync(ad => ad.Id == addressId)) throw new ObjectNotFoundException("Address not found");
                var sql = "DELETE FROM \"Address\" WHERE \"Id\" = @p0";
                int affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(sql, addressId);
                if (affectedRows == 0)
                {
                    sql = "UPDATE \"Address\" SET \"IsDeleted\" = @p0 WHERE \"Id\" = @p1";
                    await _dbContext.Database.ExecuteSqlRawAsync(sql, true, addressId);
                }
            }
            catch
            {
                throw new BusinessException("Address removal failed");
            }
        }
    } 
}
