namespace Boundless.Backend.Managers.Tokens.Models;

public class TokenDatabaseSettings : DatabaseSettings
{
    public string TokenCollectionName { get; set; } = null!;
}