namespace Boundless.Backend.Managers;

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;
    
    // Define in derived class
    // public string CollectionName { get; set; } = null!;
}