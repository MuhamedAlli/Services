using Microsoft.AspNetCore.Identity;
using ServicesApp.Infrastructure.Consts;

namespace ServicesApp.Infrastructure.Seeds
{
    public class DefaultRoles
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(AppRoles.SuperAdmin));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.User));
            }
        }
    }
}
