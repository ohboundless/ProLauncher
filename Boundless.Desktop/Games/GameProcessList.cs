using System.Diagnostics;
using System.Windows;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.Games;

public static class GameProcessList
{
    private static string[] FortniteProcessNames =
    [
        "FortniteLauncher", "FortniteClient-Win64-Shipping", "FortniteClient-Win64-Shipping_EAC_EOS"
    ];

    public static async Task FortniteProcessLoop()
    {
        while (true)
        {
            MarkIsRunning(IsFortniteRunning());
            
            await Task.Delay(5000); // 5 seconds
        }
    }

    public static void MarkIsRunning(bool isRunning)
    {
        // TODO fortnite button
        /*MainWindow.LaunchFortniteBtn.Visibility = isRunning ? Visibility.Collapsed : Visibility.Visible;
        MainWindow.CloseFortniteBtn.Visibility = isRunning ? Visibility.Visible : Visibility.Collapsed;*/
    }

    public static void CloseFortnite()
    {
        var processes = FortniteProcessNames.Select(Process.GetProcessesByName);
        foreach (var processList in processes)
        {
            foreach (var process in processList)
            {
                Log("GameProcessList_CloseFortnite", LogLevel.Info, $"Killing process: {process.ProcessName} ({process.Id})");
                process.Kill();
            }
        }
        
        MarkIsRunning(IsFortniteRunning());
    }

    public static bool IsFortniteRunning()
    {
        foreach (var name in FortniteProcessNames)
        {
            if (Process.GetProcessesByName(name).Length > 0)
                return true;
        }

        return false;
    }
}