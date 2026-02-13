using System.Data;
using System.IO;
using Boundless.Desktop.Config;
using Boundless.Desktop.Games.EpicGames.Fortnite;
using Newtonsoft.Json;

namespace Boundless.Desktop.Games;

public class AccountHolder
{
    private static string _directory => Configuration.DirectoryName + "\\Accounts"; 
    
    [JsonProperty("accounts")]
    public List<IGameAccount> Accounts { get; }
    
    public static async Task<AccountHolder> LoadAsync(string name)
    {
        var text = await File.ReadAllTextAsync(_directory + "\\" + name);
        var holder = JsonConvert.DeserializeObject<AccountHolder>(text);

        if (holder == null)
            throw new NoNullAllowedException($"Could not load the holder for '{name}'");
        
        return holder;
    }
    
    public static async Task SaveAsync(string name, AccountHolder holder)
    {
        var text = JsonConvert.SerializeObject(holder, Formatting.Indented);
        await File.WriteAllTextAsync(_directory + "\\" + name, text);
    }
}