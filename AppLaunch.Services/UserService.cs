using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppLaunch.Services.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace AppLaunch.Services;

public interface IUserService
{
    Task<List<ApplicationUser>> GetAllUsersAsync();
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
    Task<bool> UpdateUserAsync(ApplicationUser user);
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> AddUserAsync(string email, string password);
    Task<bool> UpdateUserRolesAsync(string userId, List<string> newRoles);
    string GeneratePassword();
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<ApplicationUser?> GetCurrentUserAsync();
}

public class UserService(UserManager<ApplicationUser> userManager, AuthenticationStateProvider authStateProvider) : IUserService
{
    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        return await userManager.Users.ToListAsync();
    }
    
    public async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        var authState = await authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity is not { IsAuthenticated: true })
            return null;

        return await userManager.FindByNameAsync(user.Identity.Name);
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
    
    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
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
    
    
    public string GeneratePassword()
    {
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string numbers = "0123456789";
        const string symbols = "!@#$%^&*()-_=+<>?";
        const string allChars = uppercase + lowercase + numbers + symbols;

        Random random = new();
        int length = random.Next(10, 16); // Random length between 10 and 15

        // Ensure required characters
        char[] password = new char[length];
        password[0] = uppercase[random.Next(uppercase.Length)];
        password[1] = numbers[random.Next(numbers.Length)];
        password[2] = symbols[random.Next(symbols.Length)];

        // Fill the rest with random characters
        for (int i = 3; i < length; i++)
        {
            password[i] = allChars[random.Next(allChars.Length)];
        }

        // Shuffle and return
        return new string(password.OrderBy(_ => random.Next()).ToArray());
    }
    
}