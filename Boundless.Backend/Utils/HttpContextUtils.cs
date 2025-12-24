using System.Data;

namespace Boundless.Backend.Utils;

public static class HttpContextUtils
{
    public static string GetRemoteIpAddress(this HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString()
            ?? throw new NoNullAllowedException();
    }
}