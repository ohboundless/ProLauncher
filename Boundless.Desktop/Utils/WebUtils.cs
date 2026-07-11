using System.Diagnostics;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Boundless.Desktop.Utils;

public static class WebUtils
{
    public static async Task<dynamic> GetJsonAsync(this RestClient client, RestRequest request)
    {
        Log(ELogCategory.Http, ELogLevel.Info, $"Getting JSON from '{client.BuildUri(request).AbsoluteUri}'");
        var response = await client.ExecuteAsync(request);
        var json = JObject.Parse(response.Content);

        return json;
    }

    public static dynamic GetJson(this RestClient client, RestRequest request)
    {
        return client.GetJsonAsync(request).GetAwaiter().GetResult();
    }
    
    public static void StartUrl(string url)
    {
        Log(ELogCategory.Processes, ELogLevel.Info, $"Starting url: {url}");
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}