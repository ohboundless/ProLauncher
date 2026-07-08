using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Boundless.Desktop.Games.EpicGames;
using Boundless.Desktop.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Boundless.Desktop.Games.EpicGames.Fortnite;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public static class FortniteHandler
{
    #region API HANDLER

    private static RestClient _client { get; } = new();

    private const string TOKEN_URI = "https://account-public-service-prod.ol.epicgames.com/account/api/oauth/token";

    // fortniteNewSwitchGameClient
    private const string SWITCH_CLIENT =
        "Basic OThmN2U0MmMyZTNhNGY4NmE3NGViNDNmYmI0MWVkMzk6MGEyNDQ5YTItMDAxYS00NTFlLWFmZWMtM2U4MTI5MDFjNGQ3";
    
    // launcherAppClient2
    private const string LAUNCHER_CLIENT =
        "Basic MzRhMDJjZjhmNDQxNGUyOWIxNTkyMTg3NmRhMzZmOWE6ZGFhZmJjY2M3Mzc3NDUwMzlkZmZlNTNkOTRmYzc2Y2Y=";
    
    private static async Task<string> GetClientAccessTokenAsync()
    {
        var request = new RestRequest(TOKEN_URI, Method.Post);
        request.AddHeader("Authorization", SWITCH_CLIENT);
        request.AddParameter("grant_type", "client_credentials");

        var response = await _client.ExecuteAsync(request);
        var json = JObject.Parse(response.Content);
        
        return json["access_token"]?.ToString() ?? throw new NoNullAllowedException("Could not get 'access_token'.");
    }
    
    /// <summary>
    /// Generate a device auth Uri and code
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NoNullAllowedException"></exception>
    public static async Task<(string DeviceCode, string Uri)> GetDeviceAuthAsync()
    {
        var accessToken = await GetClientAccessTokenAsync();
        var request = new RestRequest("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/deviceAuthorization", Method.Post);
        request.AddHeader("Authorization", "Bearer " + accessToken);

        var response = await _client.ExecuteAsync(request);
        var json = JObject.Parse(response.Content);
        
        return (
            json["device_code"]?.ToString() 
                ?? throw new NoNullAllowedException("Could not get 'device_code'."),
                json["verification_uri_complete"]?.ToString() 
               ?? throw new NoNullAllowedException("Could not get 'verification_uri_complete'.")
               );
    }

    public record FortniteToken
    {
        public required string Token;
        public required string AccountId;
        public required string DisplayName;
        public required string RefreshToken;
        public required DateTime RefreshExpiresAt;
    }
    
    public static async Task<FortniteToken?> GetTokenAsync(string? deviceCode = null, string? exchangeCode = null, string? refreshToken = null)
    {
        var request = new RestRequest(TOKEN_URI, Method.Post);

        if (deviceCode != null) // From Device Auth
        {
            request.AddHeader("Authorization", SWITCH_CLIENT);
            request.AddParameter("grant_type", "device_code");
            request.AddParameter("device_code", deviceCode);
        }
        else if (exchangeCode != null) // From /exchange
        {
            request.AddHeader("Authorization", LAUNCHER_CLIENT);
            request.AddParameter("grant_type", "exchange_code");
            request.AddParameter("exchange_code", exchangeCode);
        }
        else if (refreshToken != null)
        {
            request.AddHeader("Authorization", LAUNCHER_CLIENT);
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", refreshToken);
        }
        else
        {
            throw new NullReferenceException("All arguments are null.");
        }

        var response = await _client.ExecuteAsync(request);
        var json = JObject.Parse(response.Content);

        var errorCode = json["errorCode"]?.ToString();
        return errorCode switch
        {
            // No errors
            null => new FortniteToken
            {
                Token =
                    json["access_token"]?.ToString() ??
                    throw new NoNullAllowedException("Could not get 'access_token'."),
                AccountId =
                    json["account_id"]?.ToString() ??
                    throw new NoNullAllowedException("Could not get 'account_id'."),
                DisplayName =
                    json["displayName"]?.ToString() ??
                    throw new NoNullAllowedException("Could not get 'displayName'."),
                RefreshToken =
                    json["refresh_token"]?.ToString() ??
                    throw new NoNullAllowedException("Could not get 'refresh_token'."),
                RefreshExpiresAt = json["refresh_expires_at"]?.Value<DateTime>() ??
                                   throw new NoNullAllowedException("Could not get 'refresh_expires_at'.")
            },
            
            // Still pending
            "errors.com.epicgames.account.oauth.authorization_pending" => null, 
            
            // Unknown/unhandled error
            _ => throw new ApplicationException($"Unhandled error: \n{errorCode}\n{json["errorMessage"]}")
        };
    }

    public static async Task<FortniteToken> GetLauncherExchangeCode(string token)
    {
        var switchCode = await Code(token);
        var launcherResponse = await GetTokenAsync(exchangeCode: switchCode);
        var launcherCode = await Code(launcherResponse.Token);

        return launcherResponse with { Token = launcherCode };
        
        async Task<string> Code(string code)
        {
            var request = new RestRequest("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/exchange");
            request.AddHeader("Authorization", "Bearer " + code);
            var response = await _client.ExecuteAsync(request);
            var json = JObject.Parse(response.Content);

            return json["code"]?.ToString() ?? throw new NoNullAllowedException("Could not get 'code'.");
        }
    }
    
    #endregion
    
    #region LAUNCHER

    /// <summary>
    /// Launches Fortnite on a specific account.
    /// </summary>
    /// <param name="account"></param>
    /// <returns>True: Fortnite was launched successfully. False: Login has expired.</returns>
    public static async Task<bool> LaunchFortnite(FortniteAccount account)
    {
        if (DateTime.UtcNow > account.RefreshExpiresAt)
            return false;
    
        var token = await GetTokenAsync(refreshToken: account.RefreshToken);
        var accountId = token.AccountId;
    
        var launcher = await GetLauncherExchangeCode(token.Token);
        var exchangeCode = launcher.Token;

        account.RefreshToken = launcher.RefreshToken;
        account.RefreshExpiresAt = launcher.RefreshExpiresAt;

        if (!EpicInstallations.Installs.TryGetValue("Fortnite", out var install))
            throw new ApplicationException("Unable to launch Fortnite because it is not installed.");
            
        var win64 = install.Path + @"\FortniteGame\Binaries\Win64";
        
        var proc = Process.Start(new ProcessStartInfo
        {
            FileName = win64 + "\\FortniteLauncher.exe",
            WorkingDirectory = win64,
            Arguments = $"-AUTH_LOGIN=unused -AUTH_PASSWORD={exchangeCode} " +
                        $"-AUTH_TYPE=exchangecode -epicapp=Fortnite -epicenv=Prod -EpicPortal -epicuserid={accountId}"
        });

        GameList.LaunchedGames.Add(new LaunchedGame("Fortnite", proc));

        return true;
    }

    #endregion

    public static async Task StartAuthorization()
    {
        var auth = await GetDeviceAuthAsync();
        WebUtils.StartUrl(auth.Uri);

        FortniteToken? token = null;
        var retryCount = 0;
        
        while (token == null)
        {
            if (retryCount >= 18) // 3 minutes
                throw new TimeoutException("Device authorization has timed out!");
            
            await Task.Delay(10000); // 10 seconds
            token = await GetTokenAsync(auth.DeviceCode);
            retryCount++;
        }

        var account = new FortniteAccount(token);
        await Accounts.Fortnite.AddAccountAsync(account);
    }
}