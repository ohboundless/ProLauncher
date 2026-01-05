using Boundless.Blazor.Games.Fortnite;
using Boundless.Blazor.Models.Accounts;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Boundless.Blazor.Utils;

public static class BlazorUtils
{
    public static async Task<LoginSession> GetAccount(this ProtectedLocalStorage protectedLocalStorage)
    {
        var userId = await protectedLocalStorage.GetAsync<string>("userId");
        var expires = await protectedLocalStorage.GetAsync<DateTime>("expiresAt");
        var secret = await protectedLocalStorage.GetAsync<string>("secret");

        if (!userId.Success || !secret.Success || !expires.Success)
        {
            await protectedLocalStorage.Logout();
            return LoginSession.Invalid;
        }

        if (DateTime.UtcNow > expires.Value)
        {
            var nextSecret = await protectedLocalStorage.GetAsync<string>("nextSecret");
            if (!nextSecret.Success)
            {
                await protectedLocalStorage.Logout();
                return LoginSession.Invalid;
            }

            var request1 = new RestRequest("/api/account/nextSecret");
            request1.AddParameter("userId", userId);
            request1.AddParameter("nextSecret", nextSecret.Value);
            
            var response1 = await Client.ExecuteAsync(request1);
            if (response1.Content == null)
                throw new HttpRequestException("Response content for /api/account/nextSecret was null.");
            
            var nextSecretResponse = JsonConvert.DeserializeObject<TokenResponse>(response1.Content);
            
            await protectedLocalStorage.SetAsync("secret", nextSecretResponse.Secret);
            await protectedLocalStorage.SetAsync("expiresAt", nextSecretResponse.SecretExpiresAt);
            await protectedLocalStorage.SetAsync("nextSecret", nextSecretResponse.NextSecret);
        }

        var request = new RestRequest("/api/account/me");
        request.AddParameter("userId", userId);
        request.AddParameter("secret", secret);

        var response = await Client.ExecuteAsync(request);
        if (response.Content == null)
            throw new HttpRequestException("Response content for /api/account/me was null.");
        
        var account = JsonConvert.DeserializeObject<AccountResponse>(response.Content);
        return new LoginSession(account);
    }

    public static async Task Logout(this ProtectedLocalStorage protectedLocalStorage)
    {
        await protectedLocalStorage.DeleteAsync("userId");
        await protectedLocalStorage.DeleteAsync("secret");
        await protectedLocalStorage.DeleteAsync("expiresAt");
        await protectedLocalStorage.DeleteAsync("nextSecret");
    }
}