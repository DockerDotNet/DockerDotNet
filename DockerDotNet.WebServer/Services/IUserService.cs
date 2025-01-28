using DockerDotNet.WebServer.Models;

namespace DockerDotNet.WebServer.Services;

public interface  IUserService
{
    Task<User> GetUserByUsername(string username);
    Task CreateUser(User user);
}