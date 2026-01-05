global using static Boundless.Blazor.Globals;
using RestSharp;

namespace Boundless.Blazor;

public static class Globals
{
    public const string VERSION = "1.0.0";

    public const string BACKEND_URI = "http://localhost:5197";
    public static RestClient Client = new(BACKEND_URI);
}