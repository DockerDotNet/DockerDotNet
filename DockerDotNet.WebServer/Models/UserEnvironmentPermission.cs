namespace DockerDotNet.WebServer.Models;

public class UserEnvironmentPermission
{
    public long UserEnvironmentId { get; set; }
    public UserEnvironment UserEnvironment { get; set; }

    public long PermissionId { get; set; }
    public Permission Permission { get; set; }
}