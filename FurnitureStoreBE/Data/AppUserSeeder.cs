using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using Microsoft.AspNetCore.Identity;

namespace FurnitureStoreBE.Data
{
    public class AppUserSeeder
    {
        public static void SeedRootAdminUser(IServiceScope scope, WebApplication app)
        {
            ILogger logger = app.Logger;
            IConfiguration configuration = app.Configuration;
            var userName = configuration.GetValue<string>("RootAdminUser:UserName");
            var name = configuration.GetValue<string>("RootAdminUser:Name");
            var email = configuration.GetValue<string>("RootAdminUser:Email");
            var password = configuration.GetValue<string>("RootAdminUser:Password");
            var role = configuration.GetValue<string>("RootAdminUser:Role");
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (userName == null
               || name == null
               || email == null
               || password == null)
            {
                if (app.Environment.IsDevelopment())
                {
                    logger.LogWarning("Initial root user is not properly configured.");
                    return;
                }
                else
                {
                    throw new AppConfigException("Initial root user is not properly configured.");
                }
            }
            User? user = userManager.FindByNameAsync(userName).Result;
            if (user != null)
            {
                return;
            }
            var newUser = new User
            {
                Email = email,
                UserName = userName,
                FullName = name
            };
            IdentityResult createResult = userManager.CreateAsync(newUser, password).Result;
            if (!createResult.Succeeded)
            {
                throw new ApplicationException("Failed to create root user.");
            }
            IdentityRole? roleExists = roleManager.FindByNameAsync(role).Result;
            if (roleExists == null)
            {
                throw new ApplicationException($"Role {role} does not exist.");
            }
            IdentityResult roleResult = userManager.AddToRoleAsync(newUser, role).Result;
            if (!roleResult.Succeeded)
            {
                throw new ApplicationException("Failed to assign roles to root user.");
            }
            var claims = roleManager.GetClaimsAsync(roleExists).Result;
            IdentityResult claimsResult = userManager.AddClaimsAsync(newUser, claims).Result;
            if (!claimsResult.Succeeded)
            {
                throw new ApplicationException("Failed to assign claims to root user.");
            }
            logger.LogInformation("Created initial root user.");
        }
    }
}
