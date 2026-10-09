using Boundless.Backend.Managers.Errors.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Boundless.Backend.Managers.Errors;

public class ErrorManager : BaseManager
{
    private readonly IMongoCollection<ErrorEntry> _errorCollection;
    
    public ErrorManager(IOptions<ErrorDatabaseSettings> settings) : base(settings)
    {
        _errorCollection = _database!.GetCollection<ErrorEntry>(
            settings.Value.ErrorCollectionName);
    }
    
    public async Task AddAsync(ErrorEntry error) =>
        await _errorCollection.InsertOneAsync(error);

    public async Task<ErrorEntry?> GetByGuidAsync(Guid guid) =>
        await _errorCollection.Find(x => x.Guid == guid).FirstOrDefaultAsync();
}