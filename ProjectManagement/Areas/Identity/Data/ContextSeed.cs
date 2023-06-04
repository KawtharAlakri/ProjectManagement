using Microsoft.AspNetCore.Identity;
using ProjectManagement.Models;

namespace ProjectManagement.Areas.Identity.Data
{
    public class ContextSeed
    {
        
        public static async System.Threading.Tasks.Task SeedRolesAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {

                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }
        public static async System.Threading.Tasks.Task SeedAdminAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> RoleManager, ProjectManagementContext projectManagementContext)
        {
            var defaultUser = new ApplicationUser
            {
                UserName = "admin.zainab@poly.com",
                Email = "admin.zainab@poly.com",
                Name = "admin.zainab@poly.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Pa$$word123");
                    await userManager.CreateAsync(defaultUser, "Admin");
                    userManager.AddToRoleAsync(defaultUser, "Admin");
                    // Create a new instance of your "User" model
                    var customUser = new User
                    {
                        Username = defaultUser.Email,
                    };

                    // Add the new "User" instance to the dbContext and save changes
                    projectManagementContext.Users.Add(customUser);
                    await projectManagementContext.SaveChangesAsync();
                }
            }
        }
    }
}

