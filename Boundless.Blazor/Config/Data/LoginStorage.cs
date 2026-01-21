using Boundless.Blazor.Models.Accounts;
using Newtonsoft.Json;
using RestSharp;
using Serilog;

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
        if (Configuration.Login == null)
            return LoginSession.Invalid;
        
        var userId = Configuration.Login.UserId;
        var secret = Configuration.Login.Secret;
        var expires = Configuration.Login.ExpiresAt;

        if (DateTime.UtcNow > expires)
        {
            Logger.Information("Login secret expired requesting new one...");
            
            var nextSecret = Configuration.Login.NextSecret;

            var request1 = new RestRequest("/api/account/nextSecret");
            request1.AddParameter("userId", userId);
            request1.AddParameter("nextSecret", nextSecret);
            
            var response1 = await Client.ExecuteAsync(request1);
            if (response1.Content == null)
                throw new HttpRequestException("Response content for /api/account/nextSecret was null.");
            
            var nextSecretResponse = JsonConvert.DeserializeObject<TokenResponse>(response1.Content);
            
            Configuration.Login.Secret = nextSecretResponse.Secret;
            Configuration.Login.ExpiresAt = nextSecretResponse.SecretExpiresAt;
            Configuration.Login.NextSecret = nextSecretResponse.NextSecret;
            Configuration.Login.Save();
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
    
    public static async Task<LoginSession> GetAccount()
    {
        if (Configuration.Login == null)
            return LoginSession.Invalid;
        
        var userId = Configuration.Login.UserId;
        var secret = Configuration.Login.Secret;
        var expires = Configuration.Login.ExpiresAt;

        if (DateTime.UtcNow > expires)
        {
            Logger.Information("Login secret expired requesting new one...");
            
            var nextSecret = Configuration.Login.NextSecret;

            var request1 = new RestRequest("/api/account/nextSecret");
            request1.AddParameter("userId", userId);
            request1.AddParameter("nextSecret", nextSecret);
            
            var response1 = await Client.ExecuteAsync(request1);
            if (response1.Content == null)
                throw new HttpRequestException("Response content for /api/account/nextSecret was null.");
            
            var nextSecretResponse = JsonConvert.DeserializeObject<TokenResponse>(response1.Content);
            
            Configuration.Login.Secret = nextSecretResponse.Secret;
            Configuration.Login.ExpiresAt = nextSecretResponse.SecretExpiresAt;
            Configuration.Login.NextSecret = nextSecretResponse.NextSecret;
            Configuration.Login.Save();
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
}