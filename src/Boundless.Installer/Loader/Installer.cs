using System.Diagnostics;
using System.IO;
using Boundless.Installer.Windows;
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

        if ((bool)json.isUp) 
            return json;
        
        var msg = (string)json.statusMessage;
        Log("Launcher is down. Please check the discord for updates.");

        new Task(() =>
        {
            Thread.Sleep(5000);
            Process.Start(new ProcessStartInfo
            {
                FileName = (string)json.discordUrl,
                UseShellExecute = true
            });
        }).Start();
        
        _ = new MessagePopup("Launcher is down!", 
            $"{msg}\nOpening discord invite link in 5 seconds...").ShowDialog();

        throw new ApplicationException("Launcher is down.");
    }

    public async Task Install()
    {
        Log("Creating directory...");
        Directory.CreateDirectory(DirectoryPath);
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
        
        var appFiles = Directory.EnumerateFiles(AppPath, "*");
        foreach (var file in appFiles)
            File.Delete(file);

        var zips = Directory.EnumerateFiles(DirectoryPath, "*.zip", SearchOption.AllDirectories);
        foreach (var file in zips)
            File.Delete(file);

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