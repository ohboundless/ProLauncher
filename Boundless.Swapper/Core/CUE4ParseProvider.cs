using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.MappingsProvider.Usmap;
using CUE4Parse.UE4.IO;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Versions;

namespace Boundless.Swapper.Core;

public class CUE4ParseProvider
{
    public string PaksPath { get; }
    public DefaultFileProvider Provider { get; }
    public UnrealSystemProvider USystem { get; }
    
    private string _streamedChunksDirectory { get; }

    public CUE4ParseProvider(string paksPath, string streamedChunksDirectory, string mappingsPath)
    {
        PaksPath = paksPath;
        _streamedChunksDirectory = streamedChunksDirectory;

        Provider = new DefaultFileProvider(PaksPath, SearchOption.TopDirectoryOnly,
            new VersionContainer(EGame.GAME_UE5_LATEST));
        Provider.MappingsContainer = new FileUsmapTypeMappingsProvider(mappingsPath);
        USystem = new UnrealSystemProvider(this, mappingsPath);
    }

    public async Task Initialize()
    {
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

        var onDemandFiles = Directory.EnumerateFiles(PaksPath, "*.uondemandtoc");
        foreach (var file in onDemandFiles)
            Provider.RegisterVfs(file);
    }
}