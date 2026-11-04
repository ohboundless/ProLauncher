using System.Windows;
using System.Windows.Input;
using Boundless.Desktop.Config;
using Boundless.Desktop.Exceptions;
using Boundless.Desktop.Games;
using Boundless.Desktop.Games.EpicGames;
using Boundless.Desktop.Games.EpicGames.Fortnite.API;
using Boundless.Desktop.UserControls.Popups;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.Commands;

public class RemoveAccountCommand : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        RemoveAccount();
    }

    public event EventHandler? CanExecuteChanged;

    private async void RemoveAccount()
    {
        try
        {
            var currentAccount = MainWindow.ActiveAccount;
            await Accounts.Fortnite.RemoveAccountAsync(currentAccount);
            MainWindow.LoadAccounts();
            
            Log("RemoveAccountCommand", LogLevel.Info, $"Removed account: {currentAccount.Username} ({currentAccount.Id})");
        }
        catch (Exception ex)
        {
            ErrorPopup.Show("RemoveAccountCommand",
                "Failed to remove account. If this issue persists please report this to the dev team.", ex);
        }
    }
}