using System.Security.Cryptography;
using System.Text;
using Boundless.Backend.Managers.Accounts.Models;
using Boundless.Backend.Managers.Tokens.Models;
using Boundless.Backend.Utils;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Boundless.Backend.Managers.Accounts;

public class AccountManager : BaseManager
{
    private readonly IMongoCollection<UserAccount> _accountCollection;
    
    public AccountManager(IOptions<AccountDatabaseSettings> settings) : base(settings)
    {
        _accountCollection = _database!.GetCollection<UserAccount>(
            settings.Value.AccountCollectionName);
    }

    public async Task<UserAccount?> GetByUsernameAsync(string username) =>
        await _accountCollection.Find(x => x.Username == username).FirstOrDefaultAsync();
    
    public async Task<UserAccount?> GetByIdAsync(string id) =>
        await _accountCollection.Find(x => x.AccountId == id).FirstOrDefaultAsync();

    public async Task UpdateAsync(UserAccount entry) =>
        await _accountCollection.ReplaceOneAsync(x => x.Id == entry.Id, entry);
    
    public async Task<Token> CreateAccountAsync(string username, string password, HttpContext context)
    {
        var accountId = await UniqueUtils.CreateUniqueAsync<string, UserAccount?>(() =>
            StringUtils.RandomHex(32), GetByIdAsync);
        
        await _accountCollection.InsertOneAsync(new UserAccount
        {
            AccountId = accountId,
            Username = username,
            Password = password.ComputeHash()
        });

        var token = new Token(accountId, context);
        await TokenInstance!.AddAsync(token);
        
        return token;
    }
}