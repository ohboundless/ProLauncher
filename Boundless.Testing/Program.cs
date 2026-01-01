using System.Data;
using System.Diagnostics;
using Boundless.Blazor.Accounts.Fortnite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


var loginNew = false;
string? accountId = null;
string? exchangeCode = null;

start:

if (loginNew)
{
    var auth = await FortniteHandler.GetDeviceAuthAsync();
    Process.Start(new ProcessStartInfo
    {
        FileName = auth.Uri,
        UseShellExecute = true
    });

    Console.WriteLine("Press enter when you have completed the login...");
    Console.ReadLine();

    var token = await FortniteHandler.GetTokenAsync(auth.DeviceCode);
    accountId = token.AccountId;

    var launcher = await FortniteHandler.GetLauncherExchangeCode(token.Token);
    exchangeCode = launcher.Token;
    
    File.WriteAllText("refresh_token.txt", JsonConvert.SerializeObject(new
    {
        launcher.AccountId,
        launcher.DisplayName,
        launcher.RefreshToken,
        launcher.RefreshExpiresAt
    }, Formatting.Indented));
}
else
{
    var json = JObject.Parse(File.ReadAllText("refresh_token.txt"));
    if (DateTime.UtcNow > json["RefreshExpiresAt"]!.Value<DateTime>())
    {
        loginNew = true;
        goto start;
    }
    
    var token = await FortniteHandler.GetTokenAsync(refreshToken: json["RefreshToken"]!.ToString());
    accountId = token.AccountId;
    
    var launcher = await FortniteHandler.GetLauncherExchangeCode(token.Token);
    exchangeCode = launcher.Token;
    
    File.WriteAllText("refresh_token.txt", JsonConvert.SerializeObject(new
    {
        launcher.AccountId,
        launcher.DisplayName,
        launcher.RefreshToken,
        launcher.RefreshExpiresAt
    }, Formatting.Indented));
}

if (exchangeCode == null)
    throw new NoNullAllowedException();

Process.Start(new ProcessStartInfo
{
    FileName = "S:\\Fortnite\\FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe",
    WorkingDirectory = "S:\\Fortnite\\FortniteGame\\Binaries\\Win64",
    Arguments = $"-AUTH_LOGIN=unused -AUTH_PASSWORD={exchangeCode} " +
                $"-AUTH_TYPE=exchangecode -epicapp=Fortnite -epicenv=Prod -EpicPortal -epicuserid={accountId}"
});