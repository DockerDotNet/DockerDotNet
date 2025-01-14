using DockerDotNet.WebServer.Data;
using DockerDotNet.WebServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DockerDotNet.WebServer.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByUsername(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task CreateUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}
