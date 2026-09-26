using Boundless.Desktop.Games.EpicGames;
using Boundless.Swapper;

namespace Boundless.Desktop.Framework;

public class Swapper
{
    public SwapperCore Core { get; }
    
    public Swapper()
    {
        if (!EpicInstallations.Installs.TryGetValue("Fortnite", out var install))
        {
            throw new ApplicationException("Fortnite is not installed or the install has not been populated.");
        }
        
        Core = new SwapperCore(install.Path);
    }
}