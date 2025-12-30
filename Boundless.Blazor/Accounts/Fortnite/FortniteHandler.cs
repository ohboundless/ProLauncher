using System.Data;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Boundless.Blazor.Accounts.Fortnite;

public static class FortniteHandler
{
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
    
    public static async Task<string> GetTokenAsync(string? deviceCode, string? exchangeCode)
    {
        var request = new RestRequest(TOKEN_URI, Method.Post);

        if (deviceCode != null)
        {
            request.AddHeader("Authorization", SWITCH_CLIENT);
            request.AddParameter("grant_type", "device_code");
            request.AddParameter("device_code", deviceCode);
        }
        else
        {
            request.AddHeader("Authorization", LAUNCHER_CLIENT);
            request.AddParameter("grant_type", "exchange_code");
            request.AddParameter("exchange_code", exchangeCode);
        }

        var response = await _client.ExecuteAsync(request);
        var json = JObject.Parse(response.Content);
        
        return json["access_token"]?.ToString() ?? throw new NoNullAllowedException("Could not get 'access_token'.");
    }

    public static async Task<string> GetLauncherExchangeCode(string token)
    {
        var switchCode = await Code(token);
        var launcherToken = await GetTokenAsync(null, exchangeCode: switchCode);
        var launcherCode = await Code(launcherToken);

        return launcherCode;
        
        async Task<string> Code(string code)
        {
            var request = new RestRequest("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/exchange");
            request.AddHeader("Authorization", "Bearer " + code);
            var response = await _client.ExecuteAsync(request);
            var json = JObject.Parse(response.Content);

            return json["code"]?.ToString() ?? throw new NoNullAllowedException("Could not get 'code'.");
        }
    }
}