using Microsoft.Extensions.Configuration;
namespace AppLaunch.Services;

public interface IConnectionStringProvider
{
    string GetConnectionString();
    void SetConnectionString(string connectionString);
}

public class ConnectionStringProvider(IConfiguration configuration) : IConnectionStringProvider
{
    private string _connectionString;

    public string GetConnectionString()
    {
        if (!string.IsNullOrEmpty(_connectionString)) return _connectionString;
        var storedConnectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(storedConnectionString)) return storedConnectionString;
        return string.Empty;
    }
    
    public void SetConnectionString(string connectionString)
    {
        _connectionString = connectionString;
    }
}