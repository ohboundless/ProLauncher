using System.IO.Compression;
using Newtonsoft.Json.Linq;

namespace Boundless.Installer;

public class Downloader
{
    private HttpClient _httpClient;
    
    public Downloader()
    {
        _httpClient = new HttpClient();
    }

    public async Task<dynamic> GetStatus()
    {
        var text = await _httpClient.GetStringAsync(Statics.ServerUrl + "/api/info/status");
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

            if (totalBytes > 0)
            {
                var progress = (double)downloadedBytes / totalBytes * 100;
                Console.WriteLine($"{progress:F1}%");
            }
            else
            {
                Console.WriteLine($"Downloaded: {downloadedBytes:N0} bytes");
            }
        }
        
        output.Close();
        
        Console.WriteLine("Extracting...");
        await ZipFile.ExtractToDirectoryAsync(fileName, directoryPath, overwriteFiles: true);

        Console.Clear();
    }
}