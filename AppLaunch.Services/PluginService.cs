using System.IO.Compression;
using System.Net.Http.Headers;
using AppLaunch.Models;

namespace AppLaunch.Services;
using System.Text.Json;
using System;
using System.Reflection;
using System.Runtime.Loader;
using NuGet.Packaging;

public class PluginManager
{
    private Dictionary<string, PluginLoadContext> _pluginContexts = new();

    public bool IsPluginLoaded(string pluginName) => _pluginContexts.ContainsKey(pluginName);

    public void LoadPlugin(string pluginName)
    {
        if (_pluginContexts.ContainsKey(pluginName)) return;

        var pluginPath = GetPluginPath(pluginName);
        if (pluginPath == null) return;

        var context = new PluginLoadContext(pluginPath);
        context.LoadFromAssemblyPath(pluginPath);
        
        _pluginContexts[pluginName] = context;
        SaveRunningPlugins(); // Persist state
    }

    public void StopPlugin(string pluginName)
    {
        if (_pluginContexts.TryGetValue(pluginName, out var context))
        {
            context.UnloadPlugin();
            _pluginContexts.Remove(pluginName);
            SaveRunningPlugins();
        }
    }
    
    public async Task<CoreResponse> DeletePlugin(string pluginName)
    {
        CoreResponse myResponse = new();
        try
        {
            if (_pluginContexts.TryGetValue(pluginName, out var context))
            {
                context.UnloadPlugin();
                _pluginContexts.Remove(pluginName);
                SaveRunningPlugins();
            }

            // Locate plugin folder in PluginData
            var pluginsDir = Path.Combine(Environment.CurrentDirectory, "PluginData");
            var pluginFolder = Directory.GetDirectories(pluginsDir)
                .FirstOrDefault(dir => Path.GetFileName(dir).Equals(pluginName, StringComparison.OrdinalIgnoreCase));

            if (pluginFolder == null)
            {
                throw new Exception($"Plugin '{pluginName}' not found.");
            }

            try
            {
                Directory.Delete(pluginFolder, true); // Recursive deletion
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting plugin '{pluginName}': {ex.Message}");
            }

            myResponse.IsSuccess = true;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess=false;
            myResponse.Message = ex.Message;
        }
       
        return myResponse;
    }
    
    private void SaveRunningPlugins()
    {
        var runningPlugins = _pluginContexts.Keys.ToList();
        File.WriteAllText("runningPlugins.json", JsonSerializer.Serialize(runningPlugins));
    }

    private string? GetPluginPath(string pluginName)
    {
        var pluginsDir = Path.Combine(Environment.CurrentDirectory, "PluginData");
        return Directory.GetFiles(pluginsDir, "*.dll", SearchOption.AllDirectories)
            .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).Equals(pluginName, StringComparison.OrdinalIgnoreCase));
    }
    
    public List<string> GetAvailablePlugins()
    {
        var pluginsDir = Path.Combine(Environment.CurrentDirectory, "PluginData");

        return Directory.GetFiles(pluginsDir, "*.dll", SearchOption.AllDirectories)
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .ToList();
    }
    
    public void InitializePlugins()
    {
        if (!File.Exists("runningPlugins.json")) return;

        var savedPlugins = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("runningPlugins.json"));
    
        if (savedPlugins != null)
        {
            foreach (var pluginName in savedPlugins)
            {
                LoadPlugin(pluginName); // Automatically load saved plugins
            }
        }
    }
    
    public List<string> LoadRunningPlugins()
    {
        if (!File.Exists("runningPlugins.json")) return new List<string>();
        return JsonSerializer.Deserialize<List<string>>(File.ReadAllText("runningPlugins.json")) ?? new List<string>();
    }
    
    public Assembly? GetPluginAssembly(string pluginName)
    {
        var pluginPath = GetPluginPath(pluginName);
        if (pluginPath == null) return null;

        var context = new PluginLoadContext(pluginPath);
        return context.LoadFromAssemblyPath(pluginPath);
    }
    
    public List<Assembly> GetRunningPluginAssemblies(List<Assembly> existingAssemblies)
    {
        var assemblies = new List<Assembly>();

        var runningPlugins = LoadRunningPlugins(); // Get saved plugins from JSON
        foreach (var pluginName in runningPlugins)
        {
            var assembly = GetPluginAssembly(pluginName);

            if (assembly != null && !existingAssemblies.Contains(assembly) && !assemblies.Contains(assembly)) // Double-check uniqueness
            {
                assemblies.Add(assembly);
            }
        }

        return assemblies;
    }
}

public class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;
    private bool _unloading = false;

    public PluginLoadContext(string pluginPath) : base(isCollectible: true) 
    {
        // Enable unloading
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        if (_unloading) return null; // Prevent loading while unloading
        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null!;
    }

    public void UnloadPlugin()
    {
        _unloading = true;  // Prevent new loads
        this.Unload();      // Mark for garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers(); // Ensure cleanup
    }
}