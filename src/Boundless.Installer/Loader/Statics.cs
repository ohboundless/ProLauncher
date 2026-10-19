namespace Boundless.Installer.Loader;

public static class Statics
{
    public const string ServerUrl = "https://proswapper-api.azurewebsites.net";
    
    public static string DirectoryPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                          "\\BoundlessLauncher";

    public static string AppPath = DirectoryPath + "\\app";
    public static string LauncherPath = DirectoryPath + "\\launcher";
    public static string LobbyBotPath = DirectoryPath + "\\lobbybot";

    public static Action<string> Log;
    public static Action<double> Progress;
}