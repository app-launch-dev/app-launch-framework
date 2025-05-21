namespace AppLaunch.Services;

public interface IAppLaunchPlugin
{
    string Name { get; }
    string Description { get; }

    void LoadPlugin();
}