using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Boundless.Desktop.Exceptions;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.Utils;
using RestSharp;

namespace Boundless.Desktop.Games.EpicGames.Fortnite.API;

public static class FortniteHandler
{
    #region API HANDLER

    internal static RestClient _client { get; } = new();

    private const string TOKEN_URI = "https://account-public-service-prod.ol.epicgames.com/account/api/oauth/token";
    
    // fortniteNewSwitchGameClient
    private const string SWITCH_CLIENT =
        "Basic OThmN2U0MmMyZTNhNGY4NmE3NGViNDNmYmI0MWVkMzk6MGEyNDQ5YTItMDAxYS00NTFlLWFmZWMtM2U4MTI5MDFjNGQ3";
    
    // launcherAppClient2
    private const string LAUNCHER_CLIENT =
        "Basic MzRhMDJjZjhmNDQxNGUyOWIxNTkyMTg3NmRhMzZmOWE6ZGFhZmJjY2M3Mzc3NDUwMzlkZmZlNTNkOTRmYzc2Y2Y=";
    
    // fortniteAndroidGameClient
    private const string ANDROID_CLIENT =
        "Basic M2Y2OWU1NmM3NjQ5NDkyYzhjYzI5ZjFhZjA4YThhMTI6YjUxZWU5Y2IxMjIzNGY1MGE2OWVmYTY3ZWY1MzgxMmU=";
    
    private static async Task<string> GetClientAccessToken()
    {
        var request = new RestRequest(TOKEN_URI, Method.Post);
        request.AddHeader("Authorization", SWITCH_CLIENT);
        request.AddParameter("grant_type", "client_credentials");

        var json = await _client.GetJsonAsync(request);
        
        return json["access_token"]?.ToString() ?? throw new NoNullAllowedException("Could not get 'access_token'.");
    }
    
    /// <summary>
    /// Generate a device auth Uri and code
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NoNullAllowedException"></exception>
    private static async Task<(string DeviceCode, string Uri)> GetDeviceAuth()
    {
        var accessToken = await GetClientAccessToken();
        var request = new RestRequest("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/deviceAuthorization", Method.Post);
        request.AddHeader("Authorization", "Bearer " + accessToken);
        
        var json = await _client.GetJsonAsync(request);
        
        return (
            json["device_code"]?.ToString() 
                ?? throw new NoNullAllowedException("Could not get 'device_code'."),
                json["verification_uri_complete"]?.ToString() 
               ?? throw new NoNullAllowedException("Could not get 'verification_uri_complete'.")
               );
    }

    private static async Task<FortniteToken?> HandleTokenRequest(RestRequest request)
    {
        var json = await _client.GetJsonAsync(request);
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
            },

            // Still pending
            "errors.com.epicgames.account.oauth.authorization_pending" => null,

            // Unknown/unhandled error
            _ => throw new ApplicationException($"Unhandled error: \n{errorCode}\n{json["errorMessage"]}")
        };
    }

    public static async Task<FortniteToken?> GetToken(string client, string grantType, string code)
    {
        var request = new RestRequest(TOKEN_URI, Method.Post);
        request.AddHeader("Authorization", client);
        request.AddParameter("grant_type", grantType);
        request.AddParameter(grantType, code);

        return await HandleTokenRequest(request);
    }

    public static async Task<FortniteToken?> GetTokenWithDeviceAuth(FortniteDeviceAuth deviceAuth)
    {
        var request = new RestRequest(TOKEN_URI, Method.Post);
        request.AddHeader("Authorization", ANDROID_CLIENT);
        request.AddParameter("grant_type", "device_auth");
        request.AddParameter("device_id", deviceAuth.DeviceId);
        request.AddParameter("account_id", deviceAuth.AccountId);
        request.AddParameter("secret", deviceAuth.Secret);
        
        return await HandleTokenRequest(request);
    }

    // TODO summary
    public static async Task<FortniteDeviceAuth> CreateDeviceAuth(string accountId, string token)
    {
        var request = new RestRequest($"https://account-public-service-prod.ol.epicgames.com/account/api/public/account/{accountId}/deviceAuth", Method.Post);
        request.AddHeader("Authorization", $"Bearer " + token);
        
        var deviceAuth = await _client.GetJsonAsync<FortniteDeviceAuth>(request);
        return deviceAuth;
    }

    private static async Task<string> GetExchangeCode(string code)
    {
        var request =
            new RestRequest("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/exchange");
        request.AddHeader("Authorization", "Bearer " + code);
        var json = await _client.GetJsonAsync(request);

        return json["code"]?.ToString() ?? throw new NoNullAllowedException("Could not get 'code'.");
    }

    public static async Task<FortniteToken?> ExchangeToken(string forClient, string token)
    {
        var code = await GetExchangeCode(token);
        var response = await GetToken(forClient, "exchange_code", code);

        return response;
    }

    public static async Task<FortniteToken?> GetAndroidAccessToken(FortniteAccount account)
    {
        return await GetTokenWithDeviceAuth(account.DeviceAuth);
    }
    
    #endregion
    
    #region LAUNCHER
    
    public static async Task LaunchFortnite(FortniteAccount account)
    {
        Log("Fortnite", LogLevel.Info, "Starting Fortnite launch process...");

        var androidToken = await GetAndroidAccessToken(account);
        var exchanged = await ExchangeToken(LAUNCHER_CLIENT, androidToken.Token);
        var exchangeCode = await GetExchangeCode(exchanged.Token);
        
        if (!EpicInstallations.Installs.TryGetValue("Fortnite", out var install))
            throw new LaunchFortniteException("Unable to launch Fortnite because it is not installed.");
            
        var win64 = install.Path + @"\FortniteGame\Binaries\Win64";
        var fileName = win64 + "\\FortniteLauncher.exe";
        Log(LogCategory.Processes, LogLevel.Info, $"Launching {fileName}");
        
        var proc = Process.Start(new ProcessStartInfo
        {
            FileName = fileName,
            WorkingDirectory = win64,
            Arguments = $"-AUTH_LOGIN=unused -AUTH_PASSWORD={exchangeCode} " +
                        $"-AUTH_TYPE=exchangecode -epicapp=Fortnite -epicenv=Prod -EpicPortal -epicuserid={exchanged.AccountId}"
        });
        
        account.LastLaunchTime = DateTime.Now;
        await Accounts.Fortnite.SaveAsync();
    }

    #endregion

    /// <summary>
    /// Launch a DeviceAuthorization url to link Epic Games account
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    public static async Task StartAuthorization(CancellationToken cancellationToken)
    {
        Log("FortniteHandler", LogLevel.Info, "Starting device authorization...");
        var auth = await GetDeviceAuth();
        WebUtils.StartUrl(auth.Uri);

        FortniteToken? token = null;
        var retryCount = 0;
        
        while (token == null)
        {
            if (retryCount >= 18) // 3 minutes
                throw new TimeoutException("Device authorization has timed out!");
            
            await Task.Delay(10000); // 10 seconds
            token = await GetToken(SWITCH_CLIENT, "device_code", auth.DeviceCode);
            retryCount++;
            
            cancellationToken.ThrowIfCancellationRequested();
        }

        var androidToken = await ExchangeToken(ANDROID_CLIENT, token.Token);
        var deviceAuth = await CreateDeviceAuth(token.AccountId, androidToken.Token);
        var account = new FortniteAccount(token, deviceAuth);
        
        await Accounts.Fortnite.AddAccountAsync(account);
    }
}