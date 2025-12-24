using Boundless.Backend.Managers.Tokens.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Boundless.Backend.Managers.Tokens;

public class TokenManager
{
    private readonly IMongoCollection<Token> _accountCollection;
    
    public TokenManager(IOptions<TokenDatabaseSettings> settings)
    {
        var mongoClient = new MongoClient(
            settings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            settings.Value.DatabaseName);

        _accountCollection = mongoDatabase.GetCollection<Token>(
            settings.Value.TokenCollectionName);
    }
    
    public async Task AddAsync(Token token) =>
        await _accountCollection.InsertOneAsync(token);

    public async Task<Token?> GetBySecretAsync(string secret) =>
        await _accountCollection.Find(x => x.Secret == secret).FirstOrDefaultAsync();
}