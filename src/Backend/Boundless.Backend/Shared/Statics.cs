using Boundless.Backend.Managers.Accounts;
using Boundless.Backend.Managers.Errors;
using Boundless.Backend.Managers.GameAccounts;
using Boundless.Backend.Managers.Tokens;
using MongoDB.Driver;

namespace Boundless.Backend.Shared;

public static class Statics
{
    public static WebApplication? App;
    public static MongoClient? Mongo;

    public static T GetService<T>() => (T)App!.Services.GetService(typeof(T))!;

    public static AccountManager? AccountInstance;
    public static TokenManager? TokenInstance;
    public static GameAccountManager? GameInstance;
    public static ErrorManager? ErrorInstance;
    
    public static void StartLoops()
    {
        // Task.Run(..)
    }
}