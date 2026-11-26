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

    public const string BACKEND_URI = "http://201.79.44.147";
    public static RestClient Client = new(BACKEND_URI);

    public static MainWindow? MainWindow;
    public static MessagePopup? QueuedMessage { get; set; }

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
            var success = false;
            try
            {
                success = EpicInstallations.TryPopulateInstallations();
            }
            catch (FileNotFoundException ex)
            {
                Log("StartupTasks_PopulateInstallations", LogLevel.Error, ex.ToString());
            }

            if (!success)
            {
                Messages.QueueMessage("Fortnite Install Location",
                    "Could not automatically find Fortnite's install location. Please select Fortnite's install folder.",
                    "Select", () =>
                    {
                        var dialog = new OpenFolderDialog();
                        if (dialog.ShowDialog() != true)
                            return;

                        Settings.FortniteInstallPath = dialog.FolderName;
                        Settings.Save();

                        Log("StartupTasks_PopulateInstallations", LogLevel.Info,
                            $"FortniteInstallPath: {Settings.FortniteInstallPath}");
                    });
            }
        }

        // Cache
        FortniteCache.Load();
    }
}