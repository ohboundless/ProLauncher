using System.Diagnostics;

namespace LobbyBotController;

public static class BotController
{
    private static string DeviceAuthFilePath => Globals.FolderPath + "\\device_auth.json";
    private static HttpClient _httpClient = new();

    private static Process? _lobbyBotProcess;

    public static void Start(string deviceAuthJson)
    {
        _httpClient = new HttpClient();
        
        Directory.CreateDirectory(Globals.FolderPath);
        File.WriteAllText(DeviceAuthFilePath, deviceAuthJson);
        
        var npm = Process.Start(new ProcessStartInfo
        {
            WorkingDirectory = Globals.FolderPath + "\\src",
            FileName = "cmd.exe",
            Arguments = "/c npm install",
            CreateNoWindow = true
        });

        npm?.WaitForExit();
        
        _lobbyBotProcess = Process.Start(new ProcessStartInfo
        {
            WorkingDirectory = Globals.FolderPath + "\\src",
            FileName = "node",
            Arguments = "index.js",
            CreateNoWindow = true
        });
    }

    public static void Stop()
    {
        _lobbyBotProcess?.Kill();
        _lobbyBotProcess = null;
    }

    public static async Task<bool> TrySetCosmetic(string athenaType, string id)
    {
        try
        {
            await _httpClient.GetAsync($"http://localhost:7701/set?type={athenaType}&id={id}");
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}