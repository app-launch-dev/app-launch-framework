using System.Text;
using AppLaunch.Services.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using File = System.IO.File;

namespace AppLaunch.Services;

public interface IRegistrationService
{
    Task<(bool HasError, string ErrorMessage, bool IsSuccessful)> TestDatabaseConnection(
        string connectionString);

    Task CreateAppLaunchJsonFile();
    Task<bool> IsDatabaseConfigured();
    Task<bool> IRegistrationComplete();
    Task<bool> IsCMSPluginInstalled();
}

public class RegistrationService(ApplicationDbContext context, IDbContextFactory<ApplicationDbContext> contextFactory, IConfiguration configuration, IConnectionStringProvider connectionProvider) : IRegistrationService
{

    public async Task<(bool HasError, string ErrorMessage, bool IsSuccessful)> TestDatabaseConnection(
        string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return (true, "Connection string cannot be empty!", false);

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return (false, "", true);
            }
        }
        catch (SqlException ex)
        {
            return (true, $"Connection failed: {ex.Message}", false);
        }
    }

    public async Task CreateAppLaunchJsonFile()
    {
        string configFilePath = "applaunch.json";
        if (!File.Exists(configFilePath))
        {
            var defaultConfig = new
            {
                ConnectionStrings = new
                {
                    DefaultConnection = ""
                }
            };
            string defaultJson = System.Text.Json.JsonSerializer.Serialize(defaultConfig,
                new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(configFilePath, defaultJson, Encoding.UTF8);
        }
    }

    public async Task<bool> IsDatabaseConfigured()
    {
        var timeout = TimeSpan.FromSeconds(5);
        var retryDelay = TimeSpan.FromMilliseconds(200);
        var startTime = DateTime.UtcNow;

        while (DateTime.UtcNow - startTime < timeout)
        {
            try
            {
                using var context2 = contextFactory.CreateDbContext();
              
                // Check 1: File existence
                if (!File.Exists("applaunch.json"))
                {
                    await Task.Delay(retryDelay);
                    continue;
                }

                // Check 2: Connection string validity
                var connection = context.Database.GetDbConnection();
                connection.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                
                var connection2 = context2.Database.GetDbConnection();
                connectionProvider.SetConnectionString(configuration.GetConnectionString("DefaultConnection"));
                connection2.ConnectionString=configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connection.ConnectionString) || string.IsNullOrEmpty(connection2.ConnectionString))
                {
                    await Task.Delay(retryDelay);
                    continue;
                }

                return true; // Both checks passed
            }
            catch
            {
                await Task.Delay(retryDelay);
            }
        }

        return false; // Timeout after 3 seconds
    }


    public async Task<bool> IRegistrationComplete()
    {
        try
        {
            //using var context = contextFactory.CreateDbContext();

            var adminRoleId = context.Roles
                .Where(r => r.Name == "Admin")
                .Select(r => r.Id)
                .FirstOrDefault();

            if (adminRoleId != null)
            {
                var hasAdminUsers = context.UserRoles
                    .Any(ur => ur.RoleId == adminRoleId);

                return hasAdminUsers;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> IsCMSPluginInstalled()
    {
        try
        {
            string assemblyName = "Plugins.AppLaunch.CMS";
            return AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.GetName().Name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
           return false;
        }
    }
    
}