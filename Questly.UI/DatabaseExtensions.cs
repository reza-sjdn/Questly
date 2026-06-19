using Microsoft.AspNetCore.Identity;
using Questly.Data.Context;
using Questly.Data.Entities;

namespace Questly.UI;

public static class DatabaseExtensions
{
    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = scope.ServiceProvider.GetRequiredService<QuestlyDbContext>();

            // Only seed if no users exist in the database
            if (context.Users.Any())
            {
                return; // Database already has users, don't seed
            }

            // Seed Users
            List<string> usernames = ["1001", "1002", "1003"];

            foreach (var username in usernames)
            {
                if (await userManager.FindByNameAsync(username) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = username,
                        Email = $"{username}@school.edu",
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(user, "UserPass123!");
                }
            }
        }
    }
}
