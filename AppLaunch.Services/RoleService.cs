using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppLaunch.Services;

public interface IRoleService
{
    Task<List<string>> GetAllRolesAsync();
}

public class RoleService(RoleManager<IdentityRole> roleManager) : IRoleService
{
    // Get a list of all roles
    public async Task<List<string>> GetAllRolesAsync()
    {
        return await roleManager.Roles.Select(r => r.Name).ToListAsync();
    }
}
