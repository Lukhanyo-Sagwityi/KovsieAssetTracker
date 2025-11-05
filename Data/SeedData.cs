using KovsieAssetTracker.Helpers; 
using KovsieAssetTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.Extensions.Options;

namespace KovsieAssetTracker.Data
{
    public static class SeedData
        {

            public static void EnsureSeedData(IServiceScope serviceScope)
            {
            var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Database.Migrate();

            if (!context.Users.Any())
            {
                // Create password hasher (you might need to adjust based on your User class)
                var passwordHasher = new PasswordHasher<User>();

                // Create users with properly hashed passwords
                var users = new[] {
        new User {
            Name = "Tyrone",
            Email = "tyroneadmin@ufs.ac.za",
            PasswordHash = HashPassword("AdminTyrone123!"), // Replace with actual password
            Role = "admin"
        },
        new User {
            Name = "Lukhanyo",
            Email = "lukhanyoadmin@ufs.ac.za",
            PasswordHash = HashPassword("AdminLukhanyo123!"), // Replace with actual password
            Role = "admin"
        },
        new User {
            Name = "Lukhanyo",
            Email = "lukhanyoagent@ufs.ac.za", // Fixed email (was duplicate)
            PasswordHash = HashPassword("AgentLukhanyo123!"), // Replace with actual password
            Role = "agent"
        },
        new User {
            Name = "Tyrone",
            Email = "tyroneagent@ufs.ac.za", // Fixed email (had typo)
            PasswordHash = HashPassword("AgentTyrone123!"), // Replace with actual password
            Role = "agent"
        }
    };

                context.Users.AddRange(users);
                context.SaveChanges();
            }
        }
        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
    }
