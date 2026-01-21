using static Boundless.Blazor.Config.Configuration;
using Boundless.Blazor.Models.Accounts;
using Newtonsoft.Json;
using RestSharp;

namespace Boundless.Blazor.Config.Data;

public class LoginStorage : BinaryDataHolder
{
    public override string FileName => "login.dat";

    public string UserId { get; set; }
    public string Secret { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string NextSecret { get; set; }

    public static async Task<LoginSession> Login(string username, string password)
    {
        Logger.Information("Login secret expired requesting new one...");
        
        var request = new RestRequest("/api/account/token");
        request.AddParameter("username", username);
        request.AddParameter("password", password);

        var response = await Client.ExecuteAsync(request);
        ProcessTokenResponse(response);
        Logger.Information("Logged in...");
        
        return await GetAccount();
    }

    public static async Task<LoginSession> GetAccount()
    {
        if (LoginData == null)
            return LoginSession.Invalid;
        
        var userId = LoginData.UserId;
        var secret = LoginData.Secret;
        var expires = LoginData.ExpiresAt;

        if (DateTime.UtcNow > expires)
        {
            Logger.Information("Login secret expired requesting new one...");
            
            var nextSecret = LoginData.NextSecret;

            var request1 = new RestRequest("/api/account/nextSecret");
            request1.AddParameter("userId", userId);
            request1.AddParameter("nextSecret", nextSecret);
            
            var response1 = await Client.ExecuteAsync(request1);
            ProcessTokenResponse(response1);
        }

        Logger.Information("Getting account information...");
        
        var request = new RestRequest("/api/account/me");
        request.AddParameter("userId", userId);
        request.AddParameter("secret", secret);

        var response = await Client.ExecuteAsync(request);
        if (response.Content == null)
            throw new HttpRequestException("Response content for /api/account/me was null.");
        
        var account = JsonConvert.DeserializeObject<AccountResponse>(response.Content);
        Logger.Information("Received response. Returning valid session...");
        
        return new LoginSession(account);
    }

    public static void Logout()
    {
        if (LoginData == null)
            return;
            
        LoginData.Delete();
        LoginData = null;
    }

    private static void ProcessTokenResponse(RestResponse response)
    {
        if (response.Content == null)
            throw new HttpRequestException("Response content for /api/account/nextSecret was null.");
        
        var nextSecretResponse = JsonConvert.DeserializeObject<TokenResponse>(response.Content);
            
        LoginData!.Secret = nextSecretResponse!.Secret;
        LoginData.ExpiresAt = nextSecretResponse.SecretExpiresAt;
        LoginData.NextSecret = nextSecretResponse.NextSecret;
        LoginData.Save();
    }
}