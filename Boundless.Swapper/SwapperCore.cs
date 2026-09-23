using Boundless.Swapper.Core;
using Boundless.Swapper.Launcher;

namespace Boundless.Swapper;

public class SwapperCore
{
    private string _paksPath;
    private CUE4ParseProvider _provider;
    private Keychain _keychain;

    public UnrealSystemProvider USystem => _provider.USystem;
    public LaunchConfig LaunchConfig { get; } = new(@"C:\Users\Owens\AppData\Local\BoundlessLauncher\Launcher");
    
    public SwapperCore(string fortnitePath)
    {
        _paksPath = fortnitePath + @"\FortniteGame\Content\Paks";
    }

    public async Task Initialize()
    {
        // TODO chunk directory and mappings path
        _provider = new CUE4ParseProvider(_paksPath, "Chunks",
            @"C:\Users\Owens\Documents\FModel\Output\.data\mappings\++Fortnite+Release-42.20-CL-58011042_zs.usmap");
        await _provider.Initialize();

        _keychain = new Keychain();
        await _keychain.LoadKeys();
        await _provider.SubmitKeychain(_keychain);
    }
}