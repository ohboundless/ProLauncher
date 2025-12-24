global using static Boundless.Backend.Shared.Statics;
using Boundless.Backend;
using Boundless.Backend.Managers.Accounts;
using Boundless.Backend.Managers.Tokens;
using Boundless.Backend.Managers.Tokens.Models;
using static Boundless.Backend.Logging;

StartLogging("Boundless.Backend-.log");
Configuration.Initialize();

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AccountDatabaseSettings>(builder.Configuration.GetSection("AccountDatabase"));
builder.Services.AddSingleton<AccountManager>();
builder.Services.Configure<TokenDatabaseSettings>(builder.Configuration.GetSection("TokenDatabase"));
builder.Services.AddSingleton<TokenManager>();

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
TokenInstance = GetService<TokenManager>();

App.Run();