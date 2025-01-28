using DockerDotNet.WebServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DockerDotNet.WebServer.Data;

public static class AdminUserSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ensure roles and permissions are seeded first
        await RolePermissionSeeder.SeedAsync(context);

        // Check if the admin user already exists
        if (await context.Users.AnyAsync(u => u.Username == "admin"))
        {
            return; // Admin user already exists
        }
        
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

        // Create Password Hasher
        var passwordHasher = new PasswordHasher<User>();

        // Create Admin User
        var adminUser = new User
        {
            Username = "admin",
            PasswordHash = passwordHasher.HashPassword(null, "Password@123"), // Default password
            UserRoles = new List<UserRole>
            {
                new UserRole { RoleId = adminRole!.Id  }
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Add Admin User
        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();
    }
}