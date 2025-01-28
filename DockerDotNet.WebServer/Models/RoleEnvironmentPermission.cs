namespace DockerDotNet.WebServer.Models;

public class RoleEnvironmentPermission
{
    public long RoleEnvironmentId { get; set; }
    public RoleEnvironment RoleEnvironment { get; set; }

    public long PermissionId { get; set; }
    public Permission Permission { get; set; }
}