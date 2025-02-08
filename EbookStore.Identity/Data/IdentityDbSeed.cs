using EbookStore.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EbookStore.Identity.Data
{
    public static class IdentityDbSeed
    {
        public static async Task InitMembershipAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            {
                var userManager = scope.ServiceProvider.GetService<UserManager<AppUser>>();
                var roleManager = scope.ServiceProvider.GetService<RoleManager<AppRole>>();
                var configuration = scope.ServiceProvider.GetService<IConfiguration>();

                if (userManager == null || roleManager == null || configuration == null)
                {
                    throw new InvalidOperationException("Required services are not available in DI container.");
                }

                var superAdminEmail = configuration["membership:superAdminEmail"];
                var superAdminName = configuration["membership:superAdminName"];
                var superAdminSurname = configuration["membership:superAdminSurname"];
                var superAdminUsername = configuration["membership:superAdminUsername"];
                var superAdminPassword = configuration["membership:superAdminPassword"];
                var roles = configuration["membership:roles"]?.Split(",", StringSplitOptions.RemoveEmptyEntries);

                if (string.IsNullOrEmpty(superAdminEmail) || string.IsNullOrEmpty(superAdminName) ||
                    string.IsNullOrEmpty(superAdminSurname) || string.IsNullOrEmpty(superAdminUsername) ||
                    string.IsNullOrEmpty(superAdminPassword) || roles == null || roles.Length == 0)
                {
                    throw new InvalidOperationException("Required configuration values are missing.");
                }

                var user = await userManager.FindByEmailAsync(superAdminEmail);

                if (user == null)
                {
                    user = new AppUser
                    {
                        Name = superAdminName,
                        Surname = superAdminSurname,
                        Email = superAdminEmail,
                        UserName = superAdminUsername,
                        EmailConfirmed = true
                    };

                    var identityResult = await userManager.CreateAsync(user, superAdminPassword);

                    if (!identityResult.Succeeded)
                    {
                        throw new Exception("SuperAdmin user creation failed");
                    }
                }

                foreach (var roleName in roles)
                {
                    var role = await roleManager.FindByNameAsync(roleName);

                    if (role == null)
                    {
                        role = new AppRole { Name = roleName };
                        var roleResult = await roleManager.CreateAsync(role);

                        if (roleResult.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, roleName);
                        }
                    }
                    else if (!await userManager.IsInRoleAsync(user, roleName))
                    {
                        await userManager.AddToRoleAsync(user, roleName);
                    }
                }
            }
        }
    }
}