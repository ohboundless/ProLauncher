using System.Diagnostics;
using Boundless.Blazor.Accounts.Fortnite;

/*var auth = await FortniteHandler.GetDeviceAuthAsync();
Process.Start(new ProcessStartInfo
{
    FileName = auth.Uri,
    UseShellExecute = true
});

File.WriteAllText("device_auth.txt", auth.DeviceCode);*/

var deviceCode = File.ReadAllText("device_auth.txt");

Console.ReadLine();

var token = await FortniteHandler.GetTokenAsync(deviceCode, null);
var exchangeCode = await FortniteHandler.GetLauncherExchangeCode(token);

Process.Start(new ProcessStartInfo
{
    FileName = "S:\\Fortnite\\FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe",
    WorkingDirectory = "S:\\Fortnite\\FortniteGame\\Binaries\\Win64",
    Arguments = $"-AUTH_LOGIN=unused -AUTH_PASSWORD={exchangeCode} " +
                "-AUTH_TYPE=exchangecode -epicapp=Fortnite -epicenv=Prod -EpicPortal -epicuserid=efebce63db7d4fe291e804cc3dfc3222"
});