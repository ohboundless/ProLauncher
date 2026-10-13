using System.Diagnostics;
using Boundless;
using Boundless.Installer;

try
{
    foreach (var dir in Statics.Directories)
        Directory.CreateDirectory(dir);

    var keyFile = Statics.DirectoryPath + @"\Launcher\Key.txt";
    if (!File.Exists(keyFile))
    {
        Console.Write("If you would like to use the swapper please enter a key. Otherwise, just press enter. \n> ");
        var key = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(key))
        {
            File.WriteAllText(Statics.DirectoryPath + "\\Launcher\\Key.txt", key.Trim());
            Console.WriteLine("Submitted...\n");
        }
        else
        {
            Console.Clear();
        }
    }

    var versionFile = Statics.DirectoryPath + "\\Version.txt";
    var version = File.Exists(versionFile) ? Convert.ToInt32(File.ReadAllText(versionFile)) : -1;

    Console.WriteLine("Getting status...");

    var client = new Downloader();
    var json = await client.GetStatus();

    if (version == (int)json.versionNumber)
    {
        Console.WriteLine("Up to date, launching...");
        Thread.Sleep(1000);

        Launch();
        return;
    }

    var processes = Process.GetProcessesByName("Boundless.Desktop");
    if (processes.Any())
    {
        Console.WriteLine("Killing app...");
        foreach (var process in processes)
            process.Kill();
    }

    Console.WriteLine("Downloading app...");
    await client.DownloadAndExtract(Statics.DirectoryPath + "\\App", (string)json.appUrl);

    Console.WriteLine("Downloading launcher...");
    await client.DownloadAndExtract(Statics.DirectoryPath + "\\Launcher", (string)json.launcherUrl);

    Console.WriteLine("Downloading lobby bot...");
    await client.DownloadAndExtract(Statics.DirectoryPath + "\\LobbyBot", (string)json.lobbyBotUrl);

    await File.WriteAllTextAsync(versionFile, json.versionNumber.ToString());
    Launch();
}
catch (Exception ex)
{
    Console.WriteLine("Please report this to the dev team!");
    Console.WriteLine("Please report this to the dev team!");
    Console.WriteLine("Please report this to the dev team!\n");
    
    Console.WriteLine(ex);
    Console.ReadKey();
}

void Launch()
{
    Process.Start(new ProcessStartInfo
    {
        FileName = Statics.DirectoryPath + @"\App\Boundless.Desktop.exe"
    });
}