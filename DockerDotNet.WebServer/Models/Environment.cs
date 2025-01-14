using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DockerDotNet.WebServer.Models;

public class Environment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Type { get; set; } // e.g., "Docker", "Swarm", "Kubernetes"

    [Required]
    public string ConnectionString { get; set; } // Docker API endpoint or kubeconfig path

    public long GroupId { get; set; }
    public Group Group { get; set; }
}
