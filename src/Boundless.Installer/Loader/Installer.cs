using System.Diagnostics;
using System.IO;
using static Boundless.Installer.Loader.Statics;

namespace Boundless.Installer.Loader;

public class Installer
{
    private Downloader _client;

    public Installer()
    {
        _client = new Downloader();
    }
    
    private async Task<dynamic> GetStatus()
    {
        Log("Fetching status...");

        var json = await _client.GetStatus();
        return json;

    }

    public async Task Install()
    {
        Log("Creating directories...");
        Directory.CreateDirectory(AppPath);
        Directory.CreateDirectory(LauncherPath);
        Directory.CreateDirectory(LobbyBotPath);
        
        var status = await GetStatus();
        var versionFile = DirectoryPath + "\\version.txt";
        var version = File.Exists(versionFile) ? Convert.ToInt32(await File.ReadAllTextAsync(versionFile)) : -1;

        if (version == (int)status.versionNumber)
        {
            Log("Up to date, launching...");
            await Task.Delay(1000);
            return;
        }
        
        var processes = Process.GetProcessesByName("Boundless.Desktop");
        if (processes.Any())
        {
            Log("Killing app...");
            foreach (var process in processes)
                process.Kill();
        }

        Log("Downloading app...");
        await _client.DownloadAndExtract(AppPath, (string)status.appUrl);

        Log("Downloading launcher...");
        await _client.DownloadAndExtract(LauncherPath, (string)status.launcherUrl);

        Log("Downloading lobby bot...");
        await _client.DownloadAndExtract(LobbyBotPath, (string)status.lobbyBotUrl);

        await File.WriteAllTextAsync(versionFile, status.versionNumber.ToString());
        Log("Updated...");
    }
}