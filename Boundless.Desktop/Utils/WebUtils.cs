using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Boundless.Desktop.Utils;

public static class WebUtils
{
    public static async Task<string?> GetTextAsync(this RestClient client, RestRequest request)
    {
        // Log(LogCategory.Http, LogLevel.Info, $"Getting content from '{client.BuildUri(request).AbsoluteUri}'");
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }
    
    public static async Task<dynamic> GetJsonAsync(this RestClient client, RestRequest request)
    {
        Log(LogCategory.Http, LogLevel.Info, $"Getting JSON from '{client.BuildUri(request).AbsoluteUri}'");
        var text = await client.GetTextAsync(request);
        var json = JObject.Parse(text);

        return json;
    }
    
    public static async Task<T?> GetJsonAsync<T>(this RestClient client, RestRequest request)
    {
        var uri = client.BuildUri(request).AbsoluteUri;
        Log(LogCategory.Http, LogLevel.Info, $"Getting JSON from '{uri}'");
        var text = await client.GetTextAsync(request);
        if (text == null)
        {
            Log(LogCategory.Http, LogLevel.Error, $"Could not get text from '{uri}'");
            return default;
        }
        
        var json = JsonConvert.DeserializeObject<T>(text);
        return json;
    }

    public static dynamic GetJson(this RestClient client, RestRequest request)
    {
        return client.GetJsonAsync(request).GetAwaiter().GetResult();
    }
    
    public static void StartUrl(string url)
    {
        Log(LogCategory.Processes, LogLevel.Info, $"Starting url: {url}");
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}