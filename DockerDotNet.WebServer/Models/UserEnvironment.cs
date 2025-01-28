namespace DockerDotNet.WebServer.Models;

public class UserEnvironment
{
    public long UserId { get; set; }
    public User User { get; set; }

    public long EnvironmentId { get; set; }
    public Environment Environment { get; set; }

    public ICollection<UserEnvironmentPermission> Permissions { get; set; }
}