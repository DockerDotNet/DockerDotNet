namespace DockerDotNet.WebServer.Models;

public class RoleEnvironment
{
    public long RoleId { get; set; }
    public Role Role { get; set; }

    public long EnvironmentId { get; set; }
    public Environment Environment { get; set; }

    public ICollection<RoleEnvironmentPermission> Permissions { get; set; }
}