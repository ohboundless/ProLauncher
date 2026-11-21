namespace Boundless.Installer.Loader;

public static class Statics
{
    public const string Version = "1.0";
    public const string ServerUrl = "http://201.79.44.147";
    
    public static string DirectoryPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                          "\\BoundlessLauncher";

    public static string AppPath = DirectoryPath + "\\App";
    public static string LauncherPath = DirectoryPath + "\\Launcher";
    public static string LobbyBotPath = DirectoryPath + "\\LobbyBot";

    public static Action<string> Log;
    public static Action<double> Progress;
}