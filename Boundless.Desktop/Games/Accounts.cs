using Boundless.Desktop.Games.EpicGames.Fortnite;
using Microsoft.Extensions.DependencyInjection;

namespace Boundless.Desktop.Games;

public static class Accounts
{
    public static AccountHolder<FortniteAccount> Fortnite = new("Fortnite");
    
    public static void LoadAll()
    {
        throw new NotImplementedException();
    }
}