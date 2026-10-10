using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Boundless.Desktop.Config;
using Boundless.Desktop.Exceptions;
using Boundless.Desktop.Games;
using Boundless.Desktop.Games.EpicGames.Fortnite.API;
using Boundless.Desktop.UserControls.Popups;
using Boundless.Desktop.Utils;
using Boundless.Swapper.Launcher;
using LobbyBotController;
using Newtonsoft.Json;

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

        _ = new MultipleOptionsPopup(
            "Select Launch Option",
            string.Empty,
            new KeyValuePair<string, Action>("Launch Fortnite", ExecLaunchFortnite),
            new KeyValuePair<string, Action>("Launch Fortnite (Swapper)", ExecLaunchFortniteWithSwapper));
    }

    private void ExecLaunchFortnite()
    {
        LaunchFortnite();
        var username = MainWindow.ActiveAccount?.Username ?? "NULL";
        MainWindow.PopupContainer.ShowMessage("Launching...", $"Retrieving access token for \"{username}\" and launching Fortnite.", removeAllPopups: false);
    }
    
    private void ExecLaunchFortniteWithSwapper()
    {
        Log("LaunchFortniteCommand (Swapper)", LogLevel.Info, "Starting lobby bot...");
        StartLobbyBot();
        
        Log("LaunchFortniteCommand (Swapper)", LogLevel.Info, "Starting launcher...");
        StartLauncher();
        
        Log("LaunchFortniteCommand (Swapper)", LogLevel.Info, "Executing launch Fortnite process...");
        LaunchFortnite();
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
            HandleError(ex);
        }
    }

    private void StartLauncher()
    {
        try
        {
            var launchConfig = new LaunchConfig(); // Everything gets automatically loaded
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = launchConfig.LauncherDirectory,
                FileName = launchConfig.ExePath,
                CreateNoWindow = false // Might change later
            });
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
    }

    private void StartLobbyBot()
    {
        if (Accounts.Fortnite.Accounts == null)
        {
            MainWindow.PopupContainer.ShowMessage("Please Add an Account", "Cannot select accounts because accounts are null.");
            return;
        }

        new AccountSelectPopup(Accounts.Fortnite.Accounts, async void (account) =>
        {
            try
            {
                MainWindow.PopupContainer.ShowMessage("Starting Lobby Bot", $"Please wait for {account.Username} to appear online, then invite it.");
            
                var json = JsonConvert.SerializeObject(account.DeviceAuth);
                var controller = new BotController(json);
                controller.Start();

                // Wait for bot startup
                var success = false;
                while (!success)
                {
                    await Task.Delay(1000);
                
                    if (SwapperConfig.ConfigData?.CharacterId != null)
                        success = await controller.TrySetCosmetic("AthenaCharacter", SwapperConfig.ConfigData.CharacterId);

                    if (SwapperConfig.ConfigData?.BackpackId != null)
                        success = await controller.TrySetCosmetic("AthenaBackpack", SwapperConfig.ConfigData.BackpackId);

                    if (success)
                        Log("LobbyBotController", LogLevel.Info, "Set cosmetic.");
                    else
                        Log("LobbyBotController", LogLevel.Info, "Unable to set cosmetic. Retrying...");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        });
    }

    private void HandleError(Exception ex)
    {
        var error = ex.ToString();
        MainWindow.PopupContainer.ShowMessage("ERROR",
            "An error occured. Please report this to the dev team.",
            "Copy error and close",
            () => Clipboard.SetText(error));
        Log("LaunchFortniteCommand", LogLevel.Error, error);
    }
}