namespace Boundless;

public static class Statics
{
    public const string ServerUrl = "http://localhost:5197";
    
    public static string DirectoryPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                          "\\BoundlessLauncher";

    public static List<string> Directories = new()
    {
        DirectoryPath + "\\App",
        DirectoryPath + "\\Launcher",
        DirectoryPath + "\\LobbyBot",
    };
}