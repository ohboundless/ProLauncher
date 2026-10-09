using System.Data;
using System.Diagnostics;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.MappingsProvider.Usmap;
using CUE4Parse.UE4.IO;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Versions;
using Serilog;

namespace Boundless.Swapper.Core;

public class CUE4ParseProvider
{
    public string PaksPath { get; }
    public DefaultFileProvider Provider { get; }
    public UnrealSystemProvider USystem { get; private set; }
    public bool IsOnDemandTocsMounted { get; private set; }

    private string _streamedChunksDirectory;
    private readonly MappingsProvider _mappingsProvider;

    public CUE4ParseProvider(string paksPath, string streamedChunksDirectory, MappingsProvider mappingsProvider)
    {
        PaksPath = paksPath;
        _streamedChunksDirectory = streamedChunksDirectory;
        _mappingsProvider = mappingsProvider;

        CUE4Parse.Globals.LogVfsMounts = false;
        Provider = new DefaultFileProvider(PaksPath, SearchOption.TopDirectoryOnly,
            new VersionContainer(EGame.GAME_UE5_LATEST));
    }

    public async Task Initialize()
    {
        if (_mappingsProvider.Version == null)
            throw new NoNullAllowedException("Mappings have not been loaded yet!");
        
        Provider.MappingsContainer = new FileUsmapTypeMappingsProvider(_mappingsProvider.MappingsFilePath);
        USystem = new UnrealSystemProvider(this, _mappingsProvider);
        
        Provider.Initialize();
        await RegisterOnDemandTocs();
    }

    public async Task SubmitKeychain(Keychain keychain)
    {
        await Provider.SubmitKeysAsync(keychain.Keys.Select(x =>
            new KeyValuePair<FGuid, FAesKey>(
                new FGuid(x.Guid), new FAesKey(x.Key))));
    }

    private async Task RegisterOnDemandTocs()
    {
        Directory.CreateDirectory(_streamedChunksDirectory);
        Provider.OnDemandOptions = new IoStoreOnDemandOptions
        {
            ChunkHostUri = new Uri("https://egdownload.fastly-edge.com/", UriKind.Absolute),
            ChunkCacheDirectory = new DirectoryInfo(_streamedChunksDirectory),
            DownloaderClient = new HttpClient()
        };

        /*var onDemandFiles = Directory.EnumerateFiles(PaksPath, "*.uondemandtoc");
        foreach (var file in onDemandFiles)
        {
            var sw = Stopwatch.StartNew();
            Provider.RegisterVfs(file);
            sw.Stop();
            
            Logger.Information($"Mounted uondemandtoc ({file}) in {sw.ElapsedMilliseconds}ms");
        }*/

        IsOnDemandTocsMounted = true;
    }
}