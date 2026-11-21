namespace LobbyBotController;

internal static class Globals
{
    public static string FolderPath =>
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\BoundlessLauncher\LobbyBot";
}