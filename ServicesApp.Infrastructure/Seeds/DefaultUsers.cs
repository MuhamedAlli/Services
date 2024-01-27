using Microsoft.AspNetCore.Identity;

namespace ServicesApp.Infrastructure.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedUsers(UserManager<ApplicationUser> userManager)
        {
            ApplicationUser user = new()
            {
                FullName = "Mohamed Ali",
                PhoneNumber = "01123496091",
            };
        }
    }
}
