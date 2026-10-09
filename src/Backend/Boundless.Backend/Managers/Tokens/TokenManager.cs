using Boundless.Backend.Managers.Tokens.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Boundless.Backend.Managers.Tokens;

public class TokenManager : BaseManager
{
    private readonly IMongoCollection<Token> _sessionCollection;
    
    public TokenManager(IOptions<TokenDatabaseSettings> settings) : base(settings)
    {
        _sessionCollection = _database!.GetCollection<Token>(
            settings.Value.TokenCollectionName);
    }
    
    public async Task AddAsync(Token token) =>
        await _sessionCollection.InsertOneAsync(token);

    public async Task<Token?> GetBySecretAsync(string secret) =>
        await _sessionCollection.Find(x => x.Secret == secret).FirstOrDefaultAsync();
    
    public async Task<Token?> GetByNextSecretAsync(string secret) =>
        await _sessionCollection.Find(x => x.NextSecret == secret).FirstOrDefaultAsync();
    
    public async Task UpdateAsync(Token entry) =>
        await _sessionCollection.ReplaceOneAsync(x => x.Id == entry.Id, entry);
}