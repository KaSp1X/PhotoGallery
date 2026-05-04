using Microsoft.AspNetCore.Identity;

namespace PhotoGallery.Core.Infrastructure.Identity
{
    public static class RoleCreator
    {
        public static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = ["Admin", "User"];

            foreach (var role in roles)
            {
                var exists = await roleManager.RoleExistsAsync(role);

                if (!exists)
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
