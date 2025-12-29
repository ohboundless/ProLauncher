using Boundless.Backend.Managers.Accounts.Models;
using Boundless.Backend.Managers.GameAccounts.Models;
using Boundless.Backend.Managers.Tokens.Models;
using Boundless.Backend.Utils;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Boundless.Backend.Managers.GameAccounts;

public class GameAccountManager : BaseManager
{
    private readonly IMongoCollection<FortniteAccount> _fortniteCollection;
    
    public GameAccountManager(IOptions<GameAccountDatabaseSettings> settings) : base(settings)
    {
        _fortniteCollection = _database!.GetCollection<FortniteAccount>(
            settings.Value.FortniteCollectionName);
    }

    #region Fortnite
    
    public async Task<IGameAccount?> GetFortniteByIdAsync(string id) =>
        await _fortniteCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task UpdateAsync(FortniteAccount entry) =>
        await _fortniteCollection.ReplaceOneAsync(x => x.Id == entry.Id, entry);
    
    public async Task AddFortniteAsync(string accountId, string username, string refreshToken)
    {
        await _fortniteCollection.InsertOneAsync(new FortniteAccount
        {
            AccountId = accountId,
            Username = username,
            RefreshToken = refreshToken
        });
    }
    
    #endregion
}