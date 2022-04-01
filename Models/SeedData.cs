using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NotQuora.Data;

namespace NotQuora.Models
{
    public static class SeedData
    {
        public async static Task Initialize(IServiceProvider serviceProvider)
        {
            var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            List<string> roles = new List<string> { "Admin", "User", "Tester" };
            if ( !context.Roles.Any() )
            {
                foreach ( var role in roles )
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
                context.SaveChanges();
            }
         

            if ( !context.Users.Any() )
            {
                ApplicationUser seedUser = new ApplicationUser
                {
                    Email = "Admin@notquora.ca",
                    NormalizedEmail = "ADMIN@NOTQUORA.CA",
                    UserName = "Admin@notquora.ca",
                    NormalizedUserName = "ADMIN@NOTQUORA.CA",
                    EmailConfirmed = true,
                };
                var password = new PasswordHasher<ApplicationUser>();
                var pwdHashed = password.HashPassword(seedUser, "P@22word");
                seedUser.PasswordHash = pwdHashed;
                await userManager.CreateAsync(seedUser);
                await userManager.AddToRoleAsync(seedUser, "Admin");
            }

            ApplicationUser newUser = new ApplicationUser
            {
                Email = "hb2@hotmail.ca",
                NormalizedEmail = "HB2@HOTMAIL.CA",
                UserName = "hb2@hotmail.ca",
                NormalizedUserName = "HB2@HOTMAIL.CA",
                EmailConfirmed = true,
            };
            var password1 = new PasswordHasher<ApplicationUser>();
            var pwdHashed1 = password1.HashPassword(newUser, "P@22word");
            newUser.PasswordHash = pwdHashed1;
            await userManager.CreateAsync(newUser);
            await userManager.AddToRoleAsync(newUser, "User");


        }
    }
}
