using System.IO;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Boundless.Desktop.Games.EpicGames;

public record EpicInstall
{
    public string Path { get; internal set; }
    public string Version { get; internal set; }
}

public static class EpicInstallations
{
    private static string[] Apps =
    [
        "Fortnite"
        // Valorant, etc.
    ];

    public static Dictionary<string, EpicInstall> Installs { get; } = new();
    
    public static void PopulateInstallations()
    {
        var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        var launcherDataPath = $@"{programData}\Epic\UnrealEngineLauncher\LauncherInstalled.dat";
        
        Information("LauncherDataPath: " + launcherDataPath);
        
        if (!File.Exists(launcherDataPath))
            throw new FileNotFoundException("Could not find LauncherInstalled.dat");

        var launcherData = JObject.Parse(File.ReadAllText(launcherDataPath));
        var installations = launcherData["InstallationList"]
                            ?? throw new NullReferenceException("No installations found in LauncherInstalled.dat");
        
        Information("Parsed LauncherInstalled.dat ...");
        
        foreach (var installation in installations)
        {
            var appName = installation["AppName"]?.ToString() ?? 
                          throw new NullReferenceException("'AppName' was null.");;

            if (!Apps.Contains(appName))
            {
                Information($"Skipping unsupported app: '{appName}'...");
                continue;
            }

            var path = installation["InstallLocation"]?.ToString() ?? 
                       throw new NullReferenceException("'InstallLocation' was null.");
            var version = installation["AppVersion"]?.ToString() ??
                          throw new NullReferenceException("'AppVersion' was null.");

            var result = new EpicInstall { Path = path, Version = version };
            Installs[appName] = result;
                
            Information($"Populated {appName} install location.");
        }
    }
}