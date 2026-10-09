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

public class AddAccountCommand : ICommand
{
    private static bool _isActive;
    
    public bool CanExecute(object? parameter)
    {
        return !_isActive;
    }

    public void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
        {
            Log("AddAccountCommand", LogLevel.Warn, "Cannot execute.");
            return;
        }

        var cancellationSource = new CancellationTokenSource();
        AddAccount(cancellationSource.Token, new WaitingForLoginPopup(cancellationSource.Cancel));
    }

    public event EventHandler? CanExecuteChanged;

    private async void AddAccount(CancellationToken cancellationToken, WaitingForLoginPopup popup)
    {
        _isActive = true;
        
        try
        {
            await FortniteHandler.StartAuthorization(cancellationToken);
            MainWindow.LoadAccounts();
        }
        catch (OperationCanceledException)
        {
            Log("AddAccountCommand", LogLevel.Error, "Command canceled.");
        }
        catch (Exception ex)
        {
            new ErrorPopup("AddAccountCommand",
                "Failed to authorize account. If this issue persists please report this to the dev team.", ex);
        }
        finally
        {
            _isActive = false;
            popup.RemovePopup();
            MainWindow.Topmost = true;
        }
    }
}