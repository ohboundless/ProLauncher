global using static Boundless.Backend.Globals;

namespace Boundless.Backend;

public static class Globals
{
    public const string VERSION = "1.0.0";
    
    private static bool _isConnected;

    public static bool IsConnected
    {
        get => _isConnected;
        set
        {
            _isConnected = value;
            Logger.Information("Client connected.");
        }
    }
}