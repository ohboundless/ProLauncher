using System.IO;
using Boundless.Desktop.Config;
using Boundless.Desktop.Utils;
using Newtonsoft.Json;

namespace Boundless.Desktop.Games;

public class AccountHolder<T>
    where T : IGameAccount
{
    private static string _directory => Configuration.DirectoryName + "\\Accounts";

    private List<T>? _accounts { get; set; }
    public IReadOnlyList<T>? Accounts => _accounts;

    private string Name;
    
    private bool IsLoaded() => Accounts != null;
    
    public AccountHolder(string name)
    {
        Name = name;
    }

    public async Task AddAccountAsync(T account)
    {
        await LoadAsync();

        if (_accounts!.Find(x => x.Id == account.Id) != null)
        {
            MainWindow.PopupContainer.ShowMessage("Unable to add account", $"Account with username {account.Username} is already added.");
            Log("Accounts", LogLevel.Info, $"Account with id {account.Id} is already managed.");
            return;
        }
        
        _accounts!.Add(account);
        await SaveAsync();
    }

    public async Task RemoveAccountAsync(T account)
    {
        _accounts!.Remove(account);
        await SaveAsync();
    }

    public async Task LoadAsync(bool reload = false)
    {
        if (!reload && IsLoaded())
            return;
        
        _accounts = await LoadAsync<T>(Name);
    }

    public async Task SaveAsync()
    {
        await SaveAsync(Name, this);
    }

    private static async Task<List<T>?> LoadAsync<T>(string name) 
        where T : IGameAccount
    {
        var filePath = $"{_directory}\\{name}.json";
        if (!File.Exists(filePath))
        {
            var holder = new AccountHolder<T>(name) { _accounts = new() };
            await SaveAsync(name, holder);
            return holder._accounts;
        }

        var text = await File.ReadAllTextAsync($"{_directory}\\{name}.json");
        return JsonConvert.DeserializeObject<List<T>>(text);
    }
    
    public static async Task SaveAsync<T>(string name, AccountHolder<T> holder) 
        where T : IGameAccount
    {
        Log("AccountHolder", LogLevel.Info, $"Saving accounts for '{name}'");
        
        var text = JsonConvert.SerializeObject(holder.Accounts, Formatting.Indented);
        Directory.CreateDirectory(_directory);
        await File.WriteAllTextAsync($"{_directory}\\{name}.json", text);
    }
}