using System.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Boundless.Backend.Managers;

public class BaseManager
{
    internal IMongoDatabase? _database;

    protected BaseManager(IOptions<DatabaseSettings> settings)
    {
        if (Mongo is null)
            throw new NoNullAllowedException("Mongo connection is null.");
        
        _database = Mongo.GetDatabase(settings.Value.DatabaseName);
    }
}