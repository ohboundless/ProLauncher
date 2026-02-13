using Newtonsoft.Json;

namespace Boundless.Desktop.Models.Accounts;

public enum AccountType
{
    User,
    Admin,
    Owner
}

public class AccountResponse
{
    [JsonProperty("account_type")] 
    public AccountType AccountType { get; private set; } = AccountType.User;
    
    [JsonProperty("userId")]
    public string Username { get; private set; }
    
    [JsonProperty("friends")] // Ids
    public string[] Friends { get; private set; }
}