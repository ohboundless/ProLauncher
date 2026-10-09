global using static Boundless.Backend.Shared.Statics;
using Boundless.Backend;
using Boundless.Backend.Managers.Accounts;
using Boundless.Backend.Managers.Errors;
using Boundless.Backend.Managers.Errors.Models;
using Boundless.Backend.Managers.GameAccounts;
using Boundless.Backend.Managers.Tokens;
using Boundless.Backend.Managers.Tokens.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using static Boundless.Backend.Logging;

StartLogging("Boundless.Backend-.log");
Initialize();

var builder = WebApplication.CreateBuilder(args);

var mongo = builder.Configuration.GetSection("MongoDB");
var connectionString = mongo["ConnectionString"];
Mongo = new MongoClient(connectionString);

builder.Services.Configure<AccountDatabaseSettings>(builder.Configuration.GetSection("AccountDatabase"));
builder.Services.AddSingleton<AccountManager>();

builder.Services.Configure<GameAccountDatabaseSettings>(builder.Configuration.GetSection("GameDatabase"));
builder.Services.AddSingleton<GameAccountManager>();

builder.Services.Configure<TokenDatabaseSettings>(builder.Configuration.GetSection("TokenDatabase"));
builder.Services.AddSingleton<TokenManager>();

builder.Services.Configure<ErrorDatabaseSettings>(builder.Configuration.GetSection("ErrorDatabase"));
builder.Services.AddSingleton<ErrorManager>();

builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .Services.AddControllers();

App = builder.Build();

if (App.Environment.IsDevelopment())
{
    App.MapOpenApi();
}

App.MapControllers();
App.UseHttpsRedirection();

AccountInstance = GetService<AccountManager>();
GameInstance = GetService<GameAccountManager>();
TokenInstance = GetService<TokenManager>();
ErrorInstance = GetService<ErrorManager>();

App.Run();