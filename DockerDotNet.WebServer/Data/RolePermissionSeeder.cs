using DockerDotNet.WebServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DockerDotNet.WebServer.Data;
public static class RolePermissionSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if roles or permissions already exist
        if (await context.Roles.AnyAsync() || await context.Permissions.AnyAsync())
        {
            return; // Database already seeded
        }

        // Define Permissions
        var permissions = new List<Permission>
        {
            // Containers
            new Permission { Name = "ViewContainers", Description = "Can view Docker containers" },
            new Permission { Name = "StartContainer", Description = "Can start Docker containers" },
            new Permission { Name = "StopContainer", Description = "Can stop Docker containers" },
            new Permission { Name = "DeleteContainer", Description = "Can delete Docker containers" },

            // Images
            new Permission { Name = "ViewImages", Description = "Can view Docker images" },
            new Permission { Name = "PullImage", Description = "Can pull Docker images" },
            new Permission { Name = "DeleteImage", Description = "Can delete Docker images" },

            // Volumes
            new Permission { Name = "ViewVolumes", Description = "Can view Docker volumes" },
            new Permission { Name = "CreateVolume", Description = "Can create Docker volumes" },
            new Permission { Name = "DeleteVolume", Description = "Can delete Docker volumes" },

            // Networks
            new Permission { Name = "ViewNetworks", Description = "Can view Docker networks" },
            new Permission { Name = "CreateNetwork", Description = "Can create Docker networks" },
            new Permission { Name = "DeleteNetwork", Description = "Can delete Docker networks" }
        };

        // Add permissions to context
        await context.Permissions.AddRangeAsync(permissions);
        await context.SaveChangesAsync();

        // Define Roles
        var roles = new List<Role>
        {
            new Role { Name = "Admin" },
            new Role { Name = "Operator" },
            new Role { Name = "Viewer" }
        };

        // Add roles to context
        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();

        // Map Permissions to Roles
        var rolePermissions = new List<RolePermission>
        {
            // Admin - Full access
            new RolePermission { RoleId = roles.First(r => r.Name == "Admin").Id, PermissionId = permissions.First(p => p.Name == "ViewContainers").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Admin").Id, PermissionId = permissions.First(p => p.Name == "StartContainer").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Admin").Id, PermissionId = permissions.First(p => p.Name == "StopContainer").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Admin").Id, PermissionId = permissions.First(p => p.Name == "DeleteContainer").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Admin").Id, PermissionId = permissions.First(p => p.Name == "ViewImages").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Admin").Id, PermissionId = permissions.First(p => p.Name == "PullImage").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Admin").Id, PermissionId = permissions.First(p => p.Name == "DeleteImage").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Admin").Id, PermissionId = permissions.First(p => p.Name == "ViewVolumes").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Admin").Id, PermissionId = permissions.First(p => p.Name == "CreateVolume").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Admin").Id, PermissionId = permissions.First(p => p.Name == "DeleteVolume").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Admin").Id, PermissionId = permissions.First(p => p.Name == "ViewNetworks").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Admin").Id, PermissionId = permissions.First(p => p.Name == "CreateNetwork").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Admin").Id, PermissionId = permissions.First(p => p.Name == "DeleteNetwork").Id },

            // Operator - Manage access
            new RolePermission { RoleId = roles.First(r => r.Name == "Operator").Id, PermissionId = permissions.First(p => p.Name == "ViewContainers").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Operator").Id, PermissionId = permissions.First(p => p.Name == "StartContainer").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Operator").Id, PermissionId = permissions.First(p => p.Name == "StopContainer").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Operator").Id, PermissionId = permissions.First(p => p.Name == "ViewImages").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Operator").Id, PermissionId = permissions.First(p => p.Name == "PullImage").Id },

            // Viewer - View-only access
            new RolePermission { RoleId = roles.First(r => r.Name == "Viewer").Id, PermissionId = permissions.First(p => p.Name == "ViewContainers").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Viewer").Id, PermissionId = permissions.First(p => p.Name == "ViewImages").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Viewer").Id, PermissionId = permissions.First(p => p.Name == "ViewVolumes").Id },
            new RolePermission { RoleId = roles.First(r => r.Name == "Viewer").Id, PermissionId = permissions.First(p => p.Name == "ViewNetworks").Id }
        };

        // Add role-permissions to context
        await context.RolePermissions.AddRangeAsync(rolePermissions);
        await context.SaveChangesAsync();
    }
}