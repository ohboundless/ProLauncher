using System.Data;
using Boundless.Logging.Classes;
using Newtonsoft.Json.Linq;
using OffiUtils;
using OodleSharp;
using UsmapDotNet;
using ZstdSharp;

namespace Boundless.Swapper.Core;

public class MappingsProvider
{
    private HttpClient _httpClient;
    
    public string MappingsFilePath => GetMappingsPath(Version ?? throw new NoNullAllowedException("Version has not been populated."));
    public Usmap Usmap { get; private set; }
    public string? Version { get; private set; }

    public MappingsProvider()
    {
        _httpClient = new HttpClient();
    }

    public async Task DownloadMappings()
    {
        Log("MappingsProvider", LogLevel.Info, "Downloading mappings data from uedb.dev ...");
        var json = await _httpClient.GetStringAsync("https://uedb.dev/svc/api/v1/fortnite/mappings");
        Log("MappingsProvider", LogLevel.Info, "Downloaded.");
        
        dynamic obj = JObject.Parse(json);
        Log("MappingsProvider", LogLevel.Info, "Parsed json.");

        Version = obj.version.ToString();
        if (IsCached())
        {
            Log("MappingsProvider", LogLevel.Info, $"Mappings for {Version} is cached. Loading...");
            Usmap = await LoadMappings();
            return;
        }
        else
        {
            Log("MappingsProvider", LogLevel.Info, $"Mappings for {Version} is not cached. Deleting old mappings...");
            var files = Directory.EnumerateFiles(Statics.DataDirectory, "*.usmap");
            foreach (var file in files)
            {
                File.Delete(file);
                Log("MappingsProvider", LogLevel.Info, $"Deleted {file}");
            }
        }

        Log("MappingsProvider", LogLevel.Info, "Downloading ZStandard compressed mappings...");
        
        var url = (string)obj.mappings.ZStandard;
        var data = await _httpClient.GetByteArrayAsync(url);
        
        Log("MappingsProvider", LogLevel.Info, "Downloaded mappings. Saving, parsing, and returning...");
        
        await SaveMappings(data);
        Usmap = Usmap.Parse(data, UsmapOptions);
    }

    private bool IsCached() => File.Exists(MappingsFilePath);
    
    private async Task SaveMappings(byte[] bytes)
    {
        await File.WriteAllBytesAsync(MappingsFilePath, bytes);
    }

    private async Task<Usmap> LoadMappings()
    {
        var bytes = await File.ReadAllBytesAsync(MappingsFilePath);
        return Usmap.Parse(bytes, UsmapOptions);
    }

    private string GetMappingsPath(string version)
    {
        return $"{Statics.DataDirectory}\\{version}.usmap";
    }
    
    private static bool TryDecompressZstd(
        ReadOnlySpan<byte> source,
        Span<byte> destination,
        out int bytesWritten)
    {
        using var decompressor = new Decompressor();
        return decompressor.TryUnwrap(source, destination, out bytesWritten);
    }

    private static UsmapOptions UsmapOptions { get; } = new()
    {
        Decompressor = DecompressorBuilder.Default
            .Add(CompressionAlgorithm.Oodle, OodleDecompressor.TryDecompress)
            .Add(CompressionAlgorithm.Zstd, TryDecompressZstd)
            .Build()
    };
}