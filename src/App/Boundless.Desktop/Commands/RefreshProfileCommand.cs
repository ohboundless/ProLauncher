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

public class RefreshProfileCommand : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await RefreshProfile();
    }

    public event EventHandler? CanExecuteChanged;

    private async Task RefreshProfile()
    {
        if (MainWindow.ActiveAccount == null)
            return;
        
        await MainWindow.OnSetActiveAccount(MainWindow.ActiveAccount, true);
    }
}