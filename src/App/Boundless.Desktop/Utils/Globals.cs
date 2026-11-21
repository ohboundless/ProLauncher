global using static Boundless.Desktop.Utils.Globals;
global using static Boundless.Logging.Classes.Logging;
global using Boundless.Logging.Classes;
using System.IO;
using Boundless.Desktop.FortniteAPI;
using Boundless.Desktop.Config;
using Boundless.Desktop.Config.Types;
using Boundless.Desktop.Games.EpicGames;
using Boundless.Desktop.UserControls.Popups;
using Boundless.Desktop.Windows;
using Microsoft.Win32;
using RestSharp;

namespace Boundless.Desktop.Utils;

public static class Globals
{
    public const string VERSION = "1.0.0";

    public const string BACKEND_URI = "http://localhost:5197";
    public static RestClient Client = new(BACKEND_URI);

    public static MainWindow? MainWindow;

    public static MainConfig ConfigData { get; } = Configs.GetConfig<MainConfig>("Config");
    public static SwapperConfig SwapperData { get; } = Configs.GetConfig<SwapperConfig>("Swapper");
    public static Settings Settings { get; } = Configs.GetConfig<Settings>("Settings");
    
    public static void StartupTasks()
    {
        StartLogging(Configs.DirectoryName);
        
        var now = DateTime.UtcNow;
        Log(LogCategory.Startup, LogLevel.Info, $"Opened log file ({now.ToLongDateString()} - {now.ToLongTimeString()})");
        
        // Populate Fortnite install
        if (string.IsNullOrWhiteSpace(Settings.FortniteInstallPath))
        {
            try
            {
                if (!EpicInstallations.TryPopulateInstallations())
                    throw new FileNotFoundException("Cannot find Fortnite's install location");
            }
            catch (FileNotFoundException ex)
            {
                Log("StartupTasks_PopulateInstallations", LogLevel.Error, ex.ToString());

                MessagePopup.Show("Fortnite Install Location",
                    "Cannot automatically find Fortnite's install location. Please select Fortnite's install folder.",
                    "Select", () =>
                    {
                        var dialog = new OpenFolderDialog();
                        if (dialog.ShowDialog() != true)
                            return;

                        Settings.FortniteInstallPath = dialog.FolderName;
                        Settings.Save();
                    });
            }
        }

        // Cache
        FortniteCache.Load();
    }
}