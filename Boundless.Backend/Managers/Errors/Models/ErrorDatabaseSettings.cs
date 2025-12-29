namespace Boundless.Backend.Managers.Errors.Models;

public class ErrorDatabaseSettings : DatabaseSettings
{
    public string ErrorCollectionName { get; set; } = null!;
}