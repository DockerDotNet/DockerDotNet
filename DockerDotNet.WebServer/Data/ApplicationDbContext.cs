using DockerDotNet.WebServer.Models;
using Microsoft.EntityFrameworkCore;
using Environment = DockerDotNet.WebServer.Models.Environment;

namespace DockerDotNet.WebServer.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Environment> Environments { get; set; }
    public DbSet<UserEnvironment> UserEnvironments { get; set; }
    public DbSet<UserEnvironmentPermission> UserEnvironmentPermissions { get; set; }
    public DbSet<RoleEnvironment> RoleEnvironments { get; set; }
    public DbSet<RoleEnvironmentPermission> RoleEnvironmentPermissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Composite keys for relationships
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<UserPermission>()
            .HasKey(up => new { up.UserId, up.PermissionId });
        
        modelBuilder.Entity<UserEnvironment>()
            .HasKey(ue => new { ue.UserId, ue.EnvironmentId });

        modelBuilder.Entity<UserEnvironmentPermission>()
            .HasKey(uep => new { uep.UserEnvironmentId, uep.PermissionId });

        modelBuilder.Entity<RoleEnvironment>()
            .HasKey(re => new { re.RoleId, re.EnvironmentId });

        modelBuilder.Entity<RoleEnvironmentPermission>()
            .HasKey(rep => new { rep.RoleEnvironmentId, rep.PermissionId });

        base.OnModelCreating(modelBuilder);
    }
}