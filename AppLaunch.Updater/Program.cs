using System.Diagnostics;
using System.Runtime.InteropServices;

string[] packagesToUpdate = new[]
{
    "AppLaunch.Core",
    "AppLaunch.UI"
};

// Paths are relative to the updater’s location
string exeDir = AppContext.BaseDirectory;
string projectDir = Path.GetFullPath(Path.Combine(exeDir, "..", "..", "..", "AppLaunchSite"));  // Adjust this
string publishDir = Path.Combine(exeDir, "published");
string deployDir = Path.Combine(exeDir, "..", "site");  // Adjust as needed
string offlineFile = Path.Combine(deployDir, "app_offline.htm");

Console.WriteLine("💤 Taking app offline...");
File.WriteAllText(offlineFile, "<h1>Updating…</h1>");

void Run(string cmd, string args, string workingDir)
{
    var psi = new ProcessStartInfo(cmd, args)
    {
        WorkingDirectory = workingDir,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false
    };

    using var proc = Process.Start(psi)!;
    proc.WaitForExit();
    Console.WriteLine(proc.StandardOutput.ReadToEnd());
    Console.Error.WriteLine(proc.StandardError.ReadToEnd());
}

foreach (var pkg in packagesToUpdate)
{
    Console.WriteLine($"📦 Updating NuGet package: {pkg}");
    Run("dotnet", $"add package {pkg} --version latest", projectDir);
}

Run("dotnet", "restore", projectDir);
Run("dotnet", $"publish -c Release -o \"{publishDir}\"", projectDir);

Console.WriteLine("🚀 Deploying to live folder…");
CopyRecursive(new DirectoryInfo(publishDir), new DirectoryInfo(deployDir));

Console.WriteLine("🧼 Bringing app back online.");
File.Delete(offlineFile);

void CopyRecursive(DirectoryInfo source, DirectoryInfo target)
{
    foreach (var dir in source.GetDirectories())
        CopyRecursive(dir, target.CreateSubdirectory(dir.Name));

    foreach (var file in source.GetFiles())
        file.CopyTo(Path.Combine(target.FullName, file.Name), true);
}