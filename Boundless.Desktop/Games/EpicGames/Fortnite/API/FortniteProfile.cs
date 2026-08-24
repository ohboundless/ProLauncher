using System.Data;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using RestSharp;
using Boundless.Desktop.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Boundless.Desktop.Games.EpicGames.Fortnite.API;

public class FortniteProfile(FortniteAccount account)
{
    private static RestClient _client => FortniteHandler._client;
    private const string PROFILE_URI = "https://fngw-mcp-gc-livefn.ol.epicgames.com/fortnite/api/game/v2/profile/";

    [JsonIgnore] private FortniteAccount _account = account;
    [JsonIgnore] private dynamic? _athena;
    [JsonIgnore] private dynamic? _commonPublic;
    [JsonIgnore] private dynamic? _commonCore;

    private async Task<dynamic> GetProfile(string profileId)
    {
        Log("FortniteProfiles", LogLevel.Info, $"Loading profileId: '{profileId}'");
        
        var androidToken = await FortniteHandler.GetAndroidAccessToken(_account);
        if (androidToken == null)
        {
            Log("FortniteProfiles", LogLevel.Error, "Unable to get android token!");
            throw new NoNullAllowedException("Android token was null!");
        }
        
        var request = new RestRequest(PROFILE_URI + $"{_account.Id}/client/QueryProfile", Method.Post);
        request.AddHeader("Authorization", "Bearer " + androidToken.Token);
        request.AddQueryParameter("profileId", profileId);
        request.AddJsonBody("{}");

        var json = await _client.GetJsonAsync(request);
        return json;
    }

    private async Task LoadAthenaProfile() => _athena = await GetProfile("athena");
    private async Task LoadCommonPublicProfile() => _commonPublic = await GetProfile("common_public");
    private async Task LoadCommonCoreProfile() => _commonCore = await GetProfile("common_core");

    public async Task<FortniteProfileData> LoadProfileData()
    {
        Log("FortniteProfiles", LogLevel.Info, $"Loading profile data for '{_account.Id}'");

        await LoadAthenaProfile();
        await LoadCommonCoreProfile();
        
        if (_athena == null || _commonCore == null)
            throw new NoNullAllowedException("Cannot parse profile data without the profile loaded.");

        var result = new FortniteProfileData();
        
        // common_core
        var commonCore = _commonCore.profileChanges[0].profile;
        result.Currency = CalculateCurrency(commonCore.items);
        
        // athena
        var athena = _athena.profileChanges[0].profile;
        result.AccountLevel = athena.stats.attributes.accountLevel;
        result.BattlePassLevel = athena.stats.attributes.level;
        result.Cosmetics = GatherCosmetics(athena.items);

        return result;
    }

    private int CalculateCurrency(dynamic items)
    {
        var total = 0;
        foreach (JProperty child in items)
        {
            var item = child.Value;
            var templateId = item["templateId"]?.Value<string>();
            switch (templateId)
            {
                case "Currency:MtxPurchaseBonus" 
                    or "Currency:MtxGiveaway" 
                    or "Currency:MtxPurchased" 
                    or "Currency:MtxDeveloper" 
                    or "Currency:MtxComplimentary":
                    total += item["quantity"]!.Value<int>();
                    break;
                case "Currency:MtxDebt": // TODO check if works
                    total -= item["quantity"]!.Value<int>();
                    break;
            }
        }

        return total;
    }

    private List<AthenaCosmetic> GatherCosmetics(dynamic items)
    {
        Log("FortniteProfiles", LogLevel.Info, $"Gathering cosmetics for accountId: {_account.Id}");
        
        var result = new List<AthenaCosmetic>();

        foreach (JProperty child in items)
        {
            var item = child.Value;
            var templateId = item["templateId"]?.Value<string>();
            
            if (templateId!.StartsWith("AthenaCharacter")
                || templateId.StartsWith("AthenaBackpack")
                || templateId.StartsWith("AthenaPickaxe")
                || templateId.StartsWith("AthenaDance")
                || templateId.StartsWith("AthenaGlider")
                || templateId.StartsWith("AthenaItemWrap")
                || templateId.StartsWith("AthenaLoadingScreen")
                || templateId.StartsWith("CosmeticMimosa")
                || templateId.StartsWith("CosmeticShoes")
                || templateId.StartsWith("AthenaMusicPack")
                || templateId.StartsWith("AthenaSkyDiveContrail"))
            {
                var cosmetic = item.ToObject<AthenaCosmetic>();
                if (cosmetic == null)
                {
                    Log("FortniteProfiles", LogLevel.Warn, $"Unable to parse {templateId}. Skipping...");
                    continue;
                }
                
                result.Add(cosmetic);
            }
        }
        
        return result;
    }
}