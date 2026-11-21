using Boundless.Logging.Classes;
using Boundless.Swapper.Core;
using Boundless.Swapper.Cosmetics;
using Boundless.Swapper.Launcher;
using CUE4Parse.Compression;

namespace Boundless.Swapper;

public class SwapperCore
{
    private string _paksPath;
    private Keychain _keychain;

    public CUE4ParseProvider CUE4Parse { get; }
    public UnrealSystemProvider USystem => CUE4Parse.USystem;

    public LaunchConfig LaunchConfig { get; }
    public CosmeticsLoader CosmeticsLoader { get; }
    public MappingsProvider Mappings { get; }
    
    public SwapperCore(string fortnitePath)
    {
        SC = this; // Set global instance
        
        _paksPath = fortnitePath + @"\FortniteGame\Content\Paks";
        Mappings = new MappingsProvider();
        CUE4Parse = new CUE4ParseProvider(_paksPath, Statics.DataDirectory + "\\Chunks", Mappings);
        CosmeticsLoader = new CosmeticsLoader(this);
        LaunchConfig = new();
    }

    public async Task Initialize()
    {
        Log("SwapperCore", LogLevel.Info, "Initializing mappings...");
        await Mappings.DownloadMappings();
        
        Log("SwapperCore", LogLevel.Info, "Initializing CUE4Parse...");
        await CUE4Parse.Initialize();

        Log("SwapperCore", LogLevel.Info, "Loading keychain...");
        _keychain = new Keychain();
        await _keychain.LoadKeys();
        await CUE4Parse.SubmitKeychain(_keychain);

        // Oodle
        var oodlePath = Statics.DataDirectory + "\\oo2core_9_win64.dll";
        await OodleHelper.DownloadOodleDllAsync(ref oodlePath);
        await OodleHelper.InitializeAsync(oodlePath);
        
        // Detex
        // DetexHelper.Initialize("Detex.dll");
    }
}