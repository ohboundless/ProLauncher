using Boundless.Backend.Managers.Tokens.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Boundless.Backend.Managers.Tokens;

public class TokenManager : BaseManager
{
    private readonly IMongoCollection<Token> _accountCollection;
    
    public TokenManager(IOptions<TokenDatabaseSettings> settings) : base(settings)
    {
        _accountCollection = _database!.GetCollection<Token>(
            settings.Value.TokenCollectionName);
    }
    
    public async Task AddAsync(Token token) =>
        await _accountCollection.InsertOneAsync(token);

    public async Task<Token?> GetBySecretAsync(string secret) =>
        await _accountCollection.Find(x => x.Secret == secret).FirstOrDefaultAsync();
}