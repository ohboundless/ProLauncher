using System.Diagnostics;

namespace LobbyBotController;

public class BotController
{
    private string DeviceAuthFilePath => Globals.FolderPath + "\\device_auth.json";
    private HttpClient _httpClient;
    
    public BotController(string deviceAuthJson)
    {
        Directory.CreateDirectory(Globals.FolderPath);
        _httpClient = new HttpClient();
        File.WriteAllText(DeviceAuthFilePath, deviceAuthJson);
    }

    public void Start()
    {
        var npm = Process.Start(new ProcessStartInfo
        {
            WorkingDirectory = Globals.FolderPath + "\\src",
            FileName = "cmd.exe",
            Arguments = "/c npm install",
            CreateNoWindow = false
        });

        npm?.WaitForExit();
        
        Process.Start(new ProcessStartInfo
        {
            WorkingDirectory = Globals.FolderPath + "\\src",
            FileName = "cmd.exe",
            Arguments = "/c node index.js",
            CreateNoWindow = false
        });
    }

    public async Task<bool> TrySetCosmetic(string athenaType, string id)
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