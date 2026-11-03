global using static Boundless.Desktop.Utils.Globals;
global using static Boundless.Logging.Classes.Logging;
global using Boundless.Logging.Classes;
using Boundless.Desktop.FortniteAPI;
using Boundless.Desktop.Config;
using Boundless.Desktop.Config.Types;
using Boundless.Desktop.Games.EpicGames;
using Boundless.Desktop.Windows;
using RestSharp;

namespace Boundless.Desktop.Utils;

public static class Globals
{
    public const string VERSION = "1.0.0";

    public const string BACKEND_URI = "http://localhost:5197";
    public static RestClient Client = new(BACKEND_URI);

    public static MainWindow? MainWindow;

    public static MainConfig ConfigData { get; } = Configs.GetConfig<MainConfig>("config");
    public static SwapperConfig SwapperData { get; } = Configs.GetConfig<SwapperConfig>("swapper");
    public static Settings Settings { get; } = Configs.GetConfig<Settings>("settings");
    
    public static void StartupTasks()
    {
        StartLogging(Configs.DirectoryName);
        
        var now = DateTime.UtcNow;
        Log(LogCategory.Startup, LogLevel.Info, $"Opened log file ({now.ToLongDateString()} - {now.ToLongTimeString()})");
        
        // Populate Fortnite install
        EpicInstallations.PopulateInstallations();
        
        // Cache
        FortniteCache.Load();
    }
}