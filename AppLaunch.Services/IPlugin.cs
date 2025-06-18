namespace AppLaunch.Services;

public interface IPlugin
{
    string Name { get; }
    string Description { get; }
    string DbSchema { get; }
    int MajorVersion { get; }
    int MinorVersion { get; }
    void LoadPlugin();
}