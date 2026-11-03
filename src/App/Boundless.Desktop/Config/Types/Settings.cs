using Boundless.Desktop.UserControls.Popups;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.Config.Types;

public class SettingsDescription(string name, string description) : Attribute
{
    public string Name { get; } = name;
    public string Description { get; } = description;
}

public sealed class Settings() : AbstractConfigData("settings")
{
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
    public bool HideLobbyBotConsole { get; set; } = true;

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
            }
            else
            {
                DllImports.FreeConsole();
            }
        }
    }
}