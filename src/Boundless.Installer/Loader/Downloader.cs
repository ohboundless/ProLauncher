using System.IO;
using System.IO.Compression;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using static Boundless.Installer.Loader.Statics;

namespace Boundless.Installer.Loader;

public class Downloader
{
    private HttpClient _httpClient;
    
    public Downloader()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", $"ProLauncher/{Statics.Version}");
    }

    public async Task<dynamic> GetStatus()
    {
        var text = await _httpClient.GetStringAsync(ServerUrl + "/api/info/status");
        var json = JObject.Parse(text);

        return json;
    }

    public async Task DownloadAndExtract(string directoryPath, string uri)
    {
        var fileName = directoryPath + "\\" + uri.Split("/").Last().Split("?").First();
        using var response = await _httpClient.GetAsync(uri);
        
        var totalBytes = response.Content.Headers.ContentLength ?? -1;
        var downloadedBytes = 0L;

        await using var input = await response.Content.ReadAsStreamAsync();
        await using var output = File.Create(fileName);

        var buffer = new byte[81920];
        int bytesRead;

        while ((bytesRead = await input.ReadAsync(buffer)) > 0)
        {
            await output.WriteAsync(buffer.AsMemory(0, bytesRead));
            downloadedBytes += bytesRead;

            if (totalBytes <= 0) 
                continue;
            
            var progress = (double)downloadedBytes / totalBytes;
            Progress(progress);
        }
        
        output.Close();
        
        Log("Extracting...");
        await ZipFile.ExtractToDirectoryAsync(fileName, directoryPath, overwriteFiles: true);
        File.Delete(fileName);
    }
}