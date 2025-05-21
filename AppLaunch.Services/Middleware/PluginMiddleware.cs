using System.Reflection;
using System.Runtime.Loader;
using AppWithPlugin;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;

namespace AppLaunch.Services
{
  public class CoreXPluginMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CoreXPluginMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
    {
      _next = next;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Load commands from plugins.
            string[] pluginPaths = new string[]
            {
                // Paths to plugins to load.
                @"/Users/seanschroder/Projects/CoreX-CMS/CoreXStore/bin/Debug/net9.0/CoreXStore.dll"
            };

            // IEnumerable<CoreXCMS.Services.ICommand> commands = pluginPaths.SelectMany(pluginPath =>
            // {
            //     Assembly pluginAssembly = LoadPlugin(pluginPath);
            //     return CreateCommands(pluginAssembly);
            // }).ToList();
            
            Assembly pluginAssembly = LoadPlugin(pluginPaths[0]);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
      await _next(context);
    }
    
    static Assembly LoadPlugin(string relativePath)
    {
        // Navigate up to the solution root
        string root = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

        string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
        Console.WriteLine($"Loading commands from: {pluginLocation}");
        PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
    }

    static IEnumerable<AppLaunch.Services.IAppLaunchPlugin> CreateCommands(Assembly assembly)
    {
        int count = 0;

        foreach (Type type in assembly.GetTypes())
        {
            if (typeof(AppLaunch.Services.IAppLaunchPlugin).IsAssignableFrom(type))
            {
                AppLaunch.Services.IAppLaunchPlugin result = Activator.CreateInstance(type) as AppLaunch.Services.IAppLaunchPlugin;
                if (result != null)
                {
                    count++;
                    yield return result;
                }
            }
        }

        if (count == 0)
        {
            string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
            throw new ApplicationException(
                $"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
                $"Available types: {availableTypes}");
        }
    }
    
  }
}

namespace AppWithPlugin
{
    class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}