using Boundless.Desktop.Games.EpicGames.Fortnite;

namespace Boundless.Desktop.Games;

public static class Accounts
{
    public static AccountHolder<FortniteAccount> Fortnite = new("Fortnite");
    
    public static void LoadAll()
    {
        throw new NotImplementedException();
    }
}