namespace AppLaunch.Services;
using Microsoft.AspNetCore.Components;

public interface IPluginMenuProvider
{
    IEnumerable<Type> GetPluginMenus();
}

public class PluginMenuProvider : IPluginMenuProvider
{
    public IEnumerable<Type> GetPluginMenus()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IPluginMenu).IsAssignableFrom(t) && typeof(IComponent).IsAssignableFrom(t));
    }
}
