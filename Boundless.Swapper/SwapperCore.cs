using Boundless.Swapper.Core;
using Boundless.Swapper.Launcher;
using CUE4Parse.Compression;

namespace Boundless.Swapper;

public class SwapperCore
{
    private string _paksPath;
    private Keychain _keychain;

    public CUE4ParseProvider CUE4Parse { get; private set; }
    public UnrealSystemProvider USystem => CUE4Parse.USystem;
    public LaunchConfig LaunchConfig { get; } = new(@"C:\Users\Owens\AppData\Local\BoundlessLauncher\Launcher");
    
    public SwapperCore(string fortnitePath)
    {
        _paksPath = fortnitePath + @"\FortniteGame\Content\Paks";
        // TODO chunk directory and mappings path
        CUE4Parse = new CUE4ParseProvider(_paksPath, "Chunks", 
            @"C:\Users\Owens\Documents\FModel\Output\.data\mappings\++Fortnite+Release-42.20-CL-58011042_zs.usmap");
    }

    public async Task Initialize()
    {
        await CUE4Parse.Initialize();

        _keychain = new Keychain();
        await _keychain.LoadKeys();
        await CUE4Parse.SubmitKeychain(_keychain);

        var oodlePath = @"C:\Users\Owens\AppData\Local\BoundlessLauncher\oo2core_9_win64.dll"; // TODO oodle path
        await OodleHelper.DownloadOodleDllAsync(ref oodlePath);
    }
}