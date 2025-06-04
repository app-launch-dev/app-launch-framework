using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppLaunch.Services.Data;

namespace AppLaunch.Services;

public interface IUserService
{
    Task<List<ApplicationUser>> GetAllUsersAsync();
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
    Task<bool> UpdateUserAsync(ApplicationUser user);
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> AddUserAsync(string email, string password);
    Task<bool> UpdateUserRolesAsync(string userId, List<string> newRoles);
}

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        return await userManager.Users.ToListAsync();
    }

    public async Task<bool> AddUserAsync(string email, string password)
    {
        var user = new ApplicationUser { UserName = email, Email = email };
        var result = await userManager.CreateAsync(user, password);
        return result.Succeeded;
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await userManager.FindByIdAsync(userId);
    }

    public async Task<bool> UpdateUserAsync(ApplicationUser user)
    {
        var result = await userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded;
    }
    
    public async Task<bool> UpdateUserRolesAsync(string userId, List<string> newRoles)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        var currentRoles = await userManager.GetRolesAsync(user);

        // Remove old roles not in the new list
        var rolesToRemove = currentRoles.Except(newRoles);
        foreach (var role in rolesToRemove)
        {
            await userManager.RemoveFromRoleAsync(user, role);
        }

        // Add new roles not already assigned
        var rolesToAdd = newRoles.Except(currentRoles);
        foreach (var role in rolesToAdd)
        {
            await userManager.AddToRoleAsync(user, role);
        }

        return true;
    }

    
}