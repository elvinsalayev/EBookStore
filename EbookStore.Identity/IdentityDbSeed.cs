using EbookStore.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EbookStore.Identity
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

                // Xidmətlərin null olub-olmadığını yoxlamaq
                if (userManager == null || roleManager == null || configuration == null)
                {
                    throw new InvalidOperationException("Required services are not available in DI container.");
                }

                // Konfiqurasiya parametrlərinin null olub-olmadığını yoxlamaq
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

                // SuperAdmin istifadəçisini tapmaq
                var user = await userManager.FindByEmailAsync(superAdminEmail);

                if (user == null)
                {
                    // SuperAdmin istifadəçisi tapılmadıqda onu yaratmaq
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
                        // Rollar tapılmadıqda yeni rol yaratmaq
                        role = new AppRole { Name = roleName };
                        var roleResult = await roleManager.CreateAsync(role);

                        if (roleResult.Succeeded)
                        {
                            // SuperAdmin istifadəçisini həmin rola əlavə etmək
                            await userManager.AddToRoleAsync(user, roleName);
                        }
                    }
                    else if (!await userManager.IsInRoleAsync(user, roleName))
                    {
                        // İstifadəçi artıq rolunda deyilsə, əlavə etmək
                        await userManager.AddToRoleAsync(user, roleName);
                    }
                }
            }
        }
    }
}