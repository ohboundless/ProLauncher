using Boundless.Backend.Managers.Accounts.Models;
using Boundless.Backend.Managers.Tokens.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Boundless.Backend.Controllers.Account.Models;

public class AccountResponse
{
    /*[JsonProperty("account_type")] 
    public AccountType AccountType { get; private set; } = AccountType.User;
    
    [JsonProperty("userId")]
    public string Username { get; private set; }
    
    [JsonProperty("friends")] // Ids
    public string[] Friends { get; private set; }*/
    
    // TODO Statistics

    public static JsonResult Create(UserAccount account)
    {
        return new JsonResult(new
        {
            accountType = account.AccountType,
            username = account.Username,
            friends = account.Friends
        });
    }
}