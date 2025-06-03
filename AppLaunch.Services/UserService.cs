using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppLaunch.Services.Data;

namespace AppLaunch.Services;

public interface IUserService
{
    Task<List<ApplicationUser>> GetAllUsersAsync();
}

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        return await _userManager.Users.ToListAsync();
    }
}