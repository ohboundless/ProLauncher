using System.Windows;
using System.Windows.Input;
using Boundless.Desktop.Config;
using Boundless.Desktop.Exceptions;
using Boundless.Desktop.Games;
using Boundless.Desktop.Games.EpicGames;
using Boundless.Desktop.Games.EpicGames.Fortnite.API;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.Commands;

public class LaunchFortniteCommand : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return !GameProcessList.IsFortniteRunning();
    }

    public void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
        {
            Log("LaunchFortniteCommand", LogLevel.Warn, "Cannot execute.");
            return;
        }
        
        LaunchFortnite();
        var username = MainWindow.ActiveAccount?.Username ?? "NULL";
        MainWindow.PopupContainer.ShowMessage("Launching...", $"Retrieving access token for \"{username}\" and launching Fortnite.");
    }

    public event EventHandler? CanExecuteChanged;

    private async void LaunchFortnite()
    {
        try
        {
            if (!Configuration.ConfigData!.CurrentAccounts.AccountIdsByGame.TryGetValue("Fortnite", out var current))
            {
                throw new LaunchFortniteException("Cannot launch Fortnite, no active account found!");
            }

            var account = Accounts.Fortnite.Accounts?.FirstOrDefault(x => x.Id == current);
            if (account == null)
            {
                throw new LaunchFortniteException($"Unable to launch! Could not find account with id: {current}");
            }

            await FortniteHandler.LaunchFortnite(account);
            MainWindow.LoadAccounts();
        }
        catch (LaunchFortniteException ex)
        {
            MainWindow.PopupContainer.ShowMessage("ERROR", ex.Message);
        }
        catch (Exception ex)
        {
            var error = ex.ToString();
            MainWindow.PopupContainer.ShowMessage("ERROR", 
                "An error occured. Please report this to the dev team.", 
                "Copy error and close",
                () => Clipboard.SetText(error));
            Log("LaunchFortniteCommand", LogLevel.Error, error);
        }
    }
}