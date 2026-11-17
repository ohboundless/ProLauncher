using System.IO;
using Boundless.Desktop.Games.EpicGames;
using Boundless.Desktop.UserControls.Popups;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.Config.Types;

[Flags]
public enum SettingsFlags
{
    None,
    DirectorySelector,
    FileSelector,
}

public class SettingsDescription(string name, string description, SettingsFlags flags = SettingsFlags.None) : Attribute
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public SettingsFlags Flags { get; } = flags;
}

public sealed class Settings() : AbstractConfigData("settings")
{
    [SettingsDescription("Fortnite Install Directory", "The directory which contains Fortnite.",
        SettingsFlags.DirectorySelector)]
    public string FortniteInstallPath
    {
        get;
        set
        {
            const string exeName = "FortniteClient-Win64-Shipping.exe";
            
            if (File.Exists(Path.Combine(value, @"Fortnite\FortniteGame\Binaries\Win64\", exeName)))
            {
                field = Path.Combine(value, "Fortnite\\");
                SetInstall(field);
                
                return;
            }

            if (File.Exists(Path.Combine(value, @"FortniteGame\Binaries\Win64\", exeName)))
            {
                field = value;
                SetInstall(field);
                
                return;
            }

            var directories = Directory.EnumerateDirectories(value, "*", new EnumerationOptions
            {
                RecurseSubdirectories = true
            });

            foreach (var dir in directories)
            {
                if (!File.Exists(Path.Combine(dir, exeName)))
                    continue;

                var split = dir.Split(@"FortniteGame\Binaries\Win64").First();
                split = split[..^1];
                
                field = split;
                SetInstall(field);
                
                return;
            }

            void SetInstall(string path)
            {
                EpicInstallations.Installs["Fortnite"] = new EpicInstall
                {
                    Path = path,
                    Version = "UNKNOWN"
                };
            }
        }
    }

    [SettingsDescription("Hide Account Names", "Give each account a number instead of using the display name.")]
    public bool HideAccountNames
    {
        get;
        set
        {
            field = value;
            MainWindow?.LoadAccounts();
        }
    }

    [SettingsDescription("Hide Lobby Bot Console", "Hide the lobby bot console. Turn this off to allow for debugging.")]
    public bool HideLobbyBotConsole { get; set; } = false;

    [SettingsDescription("Show Logging Console (Debugging)", "Show the logging console to allow for debugging.")]
    public bool ShowLoggingConsole
    {
        get;
        set
        {
            field = value;
            if (value)
            {
                DllImports.AllocConsole();
                
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput())
                {
                    AutoFlush = true
                });

                Console.SetError(new StreamWriter(Console.OpenStandardError())
                {
                    AutoFlush = true
                });
            }
            else
            {
                DllImports.FreeConsole();
            }
        }
    }
}