using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DockerDotNet.WebServer.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Roles assigned to the user
    public ICollection<UserRole> UserRoles { get; set; }

    // Direct permissions assigned to the user
    public ICollection<UserPermission> UserPermissions { get; set; }
}