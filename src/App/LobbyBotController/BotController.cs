using System.Diagnostics;
using Boundless.Logging.Classes;
using static Boundless.Logging.Classes.Logging;

namespace LobbyBotController;

public static class BotController
{
    private static string DeviceAuthFilePath => Globals.FolderPath + "\\device_auth.json";
    private static string OwnerIdFilePath => Globals.FolderPath + "\\owner.txt";
    private static HttpClient _httpClient = new();

    private static Process? _lobbyBotProcess;

    public static async Task Start(string deviceAuthJson, string ownerAccountId, bool createNoWindow)
    {
        _httpClient = new HttpClient();
        
        Directory.CreateDirectory(Globals.FolderPath);
        await File.WriteAllTextAsync(DeviceAuthFilePath, deviceAuthJson);
        await File.WriteAllTextAsync(OwnerIdFilePath, ownerAccountId);
        
        var npm = Process.Start(new ProcessStartInfo
        {
            WorkingDirectory = Globals.FolderPath + "\\src",
            FileName = "cmd.exe",
            Arguments = "/c npm install",
            CreateNoWindow = createNoWindow
        });

        await npm.WaitForExitAsync();
        
        _lobbyBotProcess = Process.Start(new ProcessStartInfo
        {
            WorkingDirectory = Globals.FolderPath + "\\src",
            FileName = "node",
            Arguments = "index.js",
            CreateNoWindow = createNoWindow
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
            Log("LobbyBotController", LogLevel.Info, $"Set cosmetic ({athenaType}:{id})");
            
            return true;
        }
        catch (Exception ex)
        {
            Log("BotController", LogLevel.Error, ex.ToString());
            return false;
        }
    }
    
    public static async Task SetReady()
    {
        try
        {
            await _httpClient.GetAsync($"http://localhost:7701/ready");
            Log("LobbyBotController", LogLevel.Info, "Called ready.");
        }
        catch (Exception ex)
        {
            Log("BotController", LogLevel.Error, ex.ToString());
        }
    }
}