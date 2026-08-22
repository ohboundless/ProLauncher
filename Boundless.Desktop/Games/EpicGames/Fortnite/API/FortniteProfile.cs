using System.Data;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using RestSharp;
using Boundless.Desktop.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Boundless.Desktop.Games.EpicGames.Fortnite.API;

public class FortniteProfileData
{
    public int AccountLevel;
    public bool HasUpgradedBattlePass;
    public int BattlePassLevel;
    public int Currency;
}

public class FortniteProfile
{
    private static RestClient _client => FortniteHandler._client;
    private const string PROFILE_URI = "https://fngw-mcp-gc-livefn.ol.epicgames.com/fortnite/api/game/v2/profile/";

    private FortniteAccount _account;
    private dynamic? _athena;
    private dynamic? _commonPublic;
    private dynamic? _commonCore;
    
    public FortniteProfile(FortniteAccount account)
    {
        _account = account;
    }

    private async Task<dynamic> GetProfile(string profileId)
    {
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

    public async Task LoadAthenaProfile() => _athena = await GetProfile("athena");
    public async Task LoadCommonPublicProfile() => _commonPublic = await GetProfile("common_public");
    public async Task LoadCommonCoreProfile() => _commonCore = await GetProfile("common_core");

    public async Task<FortniteProfileData> GetProfileData()
    {
        if (_athena == null || _commonCore == null)
            throw new NoNullAllowedException("Cannot parse profile data without the profile loaded.");

        var result = new FortniteProfileData();
        var commonCore = _commonCore.profileChanges[0].profile;
        var items = commonCore.items;
        
        foreach (JProperty child in items)
        {
            var item = child.Value;
            var templateId = item["templateId"]?.Value<string>();
            if (templateId == "Currency:MtxPurchaseBonus"
                || templateId == "Currency:MtxGiveaway"
                || templateId == "Currency:MtxPurchased"
                || templateId == "Currency:MtxDeveloper"
                || templateId == "Currency:MtxComplimentary")
            {
                result.Currency += item["quantity"]!.Value<int>();
            }
            else if (templateId == "Currency:MtxDebt") // TODO check if works
            {
                result.Currency -= item["quantity"]!.Value<int>();
            }
        }
        
        var athena = _athena.profileChanges[0].profile;
        result.AccountLevel = athena.stats.attributes.accountLevel;
        result.BattlePassLevel = athena.stats.attributes.level;

        return result;
    }
}