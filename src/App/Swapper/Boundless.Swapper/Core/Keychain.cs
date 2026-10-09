using System.Net.Http.Json;
using Newtonsoft.Json.Linq;

namespace Boundless.Swapper.Core;

public class KeyEntry
{
    public string Guid;
    public string Key;
}

public class Keychain
{
    private const string API_URI = "https://uedb.dev/svc/api/v1/fortnite/aes";
    private HttpClient _httpClient = new();

    public List<KeyEntry> Keys { get; } = new();

    public async Task LoadKeys()
    {
        var text = await _httpClient.GetStringAsync(API_URI);
        var json = JObject.Parse(text);
        
        Keys.Add(new KeyEntry
        {
            Guid = "00000000000000000000000000000000",
            Key = json["mainKey"].ToString()
        });

        foreach (var entry in json["dynamicKeys"])
        {
            Keys.Add(new KeyEntry
            {
                Guid = entry["guid"].ToString(),
                Key = entry["key"].ToString()
            });
        }
    }
}