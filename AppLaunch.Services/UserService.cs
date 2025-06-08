using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppLaunch.Services.Data;
using Microsoft.AspNetCore.Components.Authorization;
using AppLaunch.Models;
using Microsoft.AspNetCore.Http;

namespace AppLaunch.Services;

public interface IUserService
{
    Task<CoreResponse<List<ApplicationUser>>> GetAllUsersAsync();
    Task<CoreResponse<ApplicationUser>> GetUserByIdAsync(string userId);
    Task<CoreResponse> UpdateUserAsync(ApplicationUser user);
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> UpdateUserRolesAsync(string userId, List<string> newRoles);
    string GeneratePassword();
    Task<CoreResponse<ApplicationUser>> GetUserByEmailAsync(string email);
    Task<CoreResponse<ApplicationUser?>> GetCurrentUserAsync();
    Task<CoreResponse> AddUserAsync(ApplicationUser user, string password);
}

public class UserService(UserManager<ApplicationUser> userManager, AuthenticationStateProvider authStateProvider) : IUserService
{
    public async Task<CoreResponse<List<ApplicationUser>>> GetAllUsersAsync()
    {
        CoreResponse<List<ApplicationUser>> myResponse = new();
        try
        {
            List<ApplicationUser> users = new();
            var response = await userManager.Users.ToListAsync();
            foreach (var user in response)
            {
                ApplicationUser myApplicationUser = new()
                {
                    Id = user.Id,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    UserName = user.UserName,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                };
                users.Add(myApplicationUser);
            }
            
            myResponse.Data = users;
            myResponse.IsSuccess = true;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess = false;
            myResponse.Message = ex.Message;
        }

        return myResponse;
    }
    
    public async Task<CoreResponse<ApplicationUser?>> GetCurrentUserAsync()
    {
        CoreResponse<ApplicationUser?> myResponse = new();
        try
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not { IsAuthenticated: true })
                return null;

            var response = await userManager.FindByNameAsync(user.Identity.Name);
            
            myResponse.Data = response;
            myResponse.IsSuccess = true;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess = false;
            myResponse.Message = ex.Message;
        }

        return myResponse;
    }

    public async Task<CoreResponse> AddUserAsync(ApplicationUser user, string password)
    {
        CoreResponse myResponse = new();
        try
        {
            user.UserName = user.Email;
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded) throw new Exception(result.Errors.First().Description);;
            myResponse.IsSuccess = true;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess = false;
            myResponse.Message = ex.Message;
        }

        return myResponse;
    }

    public async Task<CoreResponse<ApplicationUser>> GetUserByIdAsync(string userId)
    {
        CoreResponse<ApplicationUser> myResponse = new();
        try
        {
           var result = await userManager.FindByIdAsync(userId);
           if (result == null) throw new Exception("User not found");
           
           myResponse.Data = result;
           myResponse.IsSuccess = true;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess = false;
            myResponse.Message = ex.Message;
        }
        return myResponse;
    }
    
    public async Task<CoreResponse<ApplicationUser>> GetUserByEmailAsync(string email)
    {
        CoreResponse<ApplicationUser> myResponse = new();
        try
        {
            var result = await userManager.FindByEmailAsync(email);
            if (result == null) throw new Exception("User not found");
            
            myResponse.Data = result;
            myResponse.IsSuccess = true;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess = false;
            myResponse.Message = ex.Message;
        }

        return myResponse;
    }
    
    public async Task<CoreResponse> UpdateUserAsync(ApplicationUser user)
    {
        CoreResponse myResponse = new();
        
        try
        {
            if(user == null) 
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            
            if(string.IsNullOrWhiteSpace(user.Email)) 
                throw new ArgumentException("Email cannot be null or whitespace.", nameof(user.Email));
            
            string? newSanitizedEmail = SanitizationExtensions.SanitizeEmail(user.Email);

            var existingUser = await userManager.FindByIdAsync(user.Id);
            if (existingUser == null)
            {
                myResponse.IsSuccess = false;
                myResponse.Message = "User to update not found.";
                return myResponse;
            }
            
            bool emailIsChanging = !string.Equals(existingUser.Email, newSanitizedEmail, StringComparison.OrdinalIgnoreCase);
            if (emailIsChanging)
            {
                var userWithNewEmail = await userManager.FindByEmailAsync(newSanitizedEmail);
                if (userWithNewEmail != null && userWithNewEmail.Id != existingUser.Id)
                {
                    throw new Exception($"Email {newSanitizedEmail} is already in use by another account.");
                }
            }
            
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.MiddleName = user.MiddleName;
            existingUser.Email = newSanitizedEmail;
            existingUser.UserName = newSanitizedEmail;
            existingUser.PhoneNumber = SanitizationExtensions.SanitizePhoneNumber(user.PhoneNumber);

            var result = await userManager.UpdateAsync(existingUser);
            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Error updating user: {errors}");
            }
            myResponse.IsSuccess = true;
            myResponse.Message = "User updated successfully.";
            
            
            // var emailExists = await GetUserByEmailAsync(user.Email);
            // if (emailExists.IsSuccess == false)
            // {
            //     if (emailExists.Data == null)
            //         user.UserName = user.Email;
            // }
            // if (emailExists != null && emailExists.Data.Id != user.Id) 
            //     throw new Exception("Email already in use");
            //
            // var result = await userManager.UpdateAsync(user);
            // if (!result.Succeeded) 
            //     throw new Exception("Error updating user");
            //
            // myResponse.IsSuccess = true;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess = false;
            myResponse.Message = ex.Message;
        }
        return myResponse;
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
    
    public ApplicationUser SanitizeUser(ApplicationUser user)
    {
        user.UserName = SanitizationExtensions.SanitizeEmail(user.Email);
        user.Email = SanitizationExtensions.SanitizeEmail(user.Email);
        user.PhoneNumber = SanitizationExtensions.SanitizePhoneNumber(user.PhoneNumber);
        return user;
    }
    
    public static class SanitizationExtensions
    {
        /// <summary>
        /// Removes all non-digit characters from the string.
        /// </summary>
        public static string? SanitizePhoneNumber(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var sanitized = string.Concat(input.Where(char.IsDigit));

            return string.IsNullOrEmpty(sanitized) ? null : sanitized;
        }
        
        /// <summary>
        /// Trims and return the lowercase equivalent of the current string.
        /// </summary>
        public static string? SanitizeEmail(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Email cannot be null or whitespace.", nameof(input));

            return input.Trim().ToLowerInvariant();
        }

    }
    
}