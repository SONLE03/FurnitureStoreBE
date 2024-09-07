using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Response.UserResponse;

using AutoMapper;
using Microsoft.AspNetCore.Identity;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Services.Caching;
using System.Security.Claims;
namespace FurnitureStoreBE.Services.UserService
{
    public class UserServiceImp : IUserService
    {
        private readonly ApplicationDBContext dbContext;
        //private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IRedisCacheService redisCacheService;
        private readonly ILogger<User> logger;
        public UserServiceImp(ApplicationDBContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager
            , IRedisCacheService redisCacheService, ILogger<User> logger)
        {
            this.dbContext = dbContext;
            //this.mapper = mapper;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.redisCacheService = redisCacheService;
            this.logger = logger;
        }
        
        public async Task<TypeClaimsReponse> GetTypeClaimsByRole(string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
                throw new ObjectNotFoundException("Role not found");
            var roleClaims = await roleManager.GetClaimsAsync(role);
            await redisCacheService.SetData("claims", roleClaims);
            foreach (var claims in roleClaims)
            {
                logger.LogInformation(claims.Value);

            }
            return null;
        }
    }
}
