using System.IO;
using System.Windows.Input;
using Boundless.Desktop.Exceptions;
using Boundless.Desktop.Games;
using Boundless.Desktop.Games.EpicGames.Fortnite.API;
using Boundless.Desktop.UserControls.Popups;
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
        
        var username = MainWindow.ActiveAccount.Username ?? "NULL";
        MessagePopup.Show("Launching...", $"Retrieving access token for \"{username}\" and launching Fortnite.");
    }
    
    private async void ExecLaunchFortniteWithSwapper()
    {
        try
        {
            Log("LaunchFortniteCommand (Swapper)", LogLevel.Info, "Starting launcher...");
            await StartLauncher();
        
            Log("LaunchFortniteCommand (Swapper)", LogLevel.Info, "Starting lobby bot...");
            await StartLobbyBot();
        
            Log("LaunchFortniteCommand (Swapper)", LogLevel.Info, "Executing launch Fortnite process...");
            LaunchFortnite();
        }
        catch (Exception ex)
        {
            ErrorPopup.Show("LaunchFortniteCommand_ExecLaunchFortniteWithSwapper",
                "Could not start Fortnite with swapper! If this error persists please report it to the dev team!", ex);
        }
    }

    public event EventHandler? CanExecuteChanged;

    private async void LaunchFortnite()
    {
        try
        {
            if (!ConfigData.CurrentAccounts.AccountIdsByGame.TryGetValue("Fortnite", out var current))
            {
                throw new LaunchFortniteException("Cannot launch Fortnite, no active account found!");
            }

            var account = Accounts.Fortnite.Accounts?.FirstOrDefault(x => x.Id == current);
            if (account == null)
            {
                throw new LaunchFortniteException($"Unable to launch! Could not find account with id: {current}");
            }

            await FortniteHandler.LaunchFortnite(account);
            MainWindow!.LoadAccounts();
        }
        catch (LaunchFortniteException ex)
        {
            MessagePopup.Show("ERROR", ex.Message);
        }
        catch (Exception ex)
        {
            ErrorPopup.Show("LaunchFortniteCommand",
                "An error occured. Please report this to the dev team.", ex);
        }
    }

    private async Task StartLauncher()
    {
        try
        {
            var launcherProc = Launcher.Launch();
            if (launcherProc != null)
            {
                launcherProc.EnableRaisingEvents = true;
                launcherProc.Exited += (_, _) => BotController.Stop();
            }

            await Task.Delay(5000); // Wait for a possibly invalid key to reset
            while (!File.Exists(LaunchConfig.LauncherDirectory + "\\key.txt"))
            {
                await Task.Delay(1000);
            }
        }
        catch (Exception ex)
        {
            ErrorPopup.Show("LaunchFortniteCommand",
                "An error occured. Please report this to the dev team.", ex);
        }
    }

    public static async Task StartLobbyBot()
    {
        if (Accounts.Fortnite.Accounts == null)
        {
            MessagePopup.Show("Please Add an Account",
                "Cannot select accounts because accounts are null.");
            return;
        }

        _ = new AccountSelectPopup(Accounts.Fortnite.Accounts, async void (account) =>
        {
            try
            {
                MessagePopup.Show("Starting Lobby Bot",
                    $"Please wait for {account.Username} to appear online, then invite it.");

                if (!ConfigData.CurrentAccounts.AccountIdsByGame.TryGetValue("Fortnite", out var current))
                    throw new LaunchFortniteException("Cannot start lobby bot, no active account found!");

                var json = JsonConvert.SerializeObject(account.DeviceAuth, Formatting.Indented);
                await BotController.Start(json, current, Settings.HideLobbyBotConsole);

                // Wait for bot startup
                var characterSuccess = false;
                var backpackSuccess = false;
                var pickaxeSuccess = false;
                var emoteSuccess = false;
                while (!characterSuccess && SwapperData.CharacterId != null
                       || !backpackSuccess && SwapperData.BackpackId != null
                       || !pickaxeSuccess && SwapperData.PickaxeId != null
                       || !emoteSuccess && SwapperData.EmoteId != null)
                {
                    await Task.Delay(1000);

                    if (SwapperData.CharacterId != null)
                    {
                        characterSuccess = await BotController.TrySetCosmetic("AthenaCharacter",
                            SwapperData.CharacterId);
                    }

                    if (SwapperData.BackpackId != null)
                    {
                        backpackSuccess =
                            await BotController.TrySetCosmetic("AthenaBackpack", SwapperData.BackpackId);
                    }

                    if (SwapperData.PickaxeId != null)
                    {
                        pickaxeSuccess =
                            await BotController.TrySetCosmetic("AthenaPickaxe", SwapperData.PickaxeId);
                    }

                    if (SwapperData.EmoteId != null)
                    {
                        emoteSuccess =
                            await BotController.TrySetCosmetic("AthenaDance", SwapperData.EmoteId);
                    }
                }

                await BotController.SetReady();
            }
            catch (Exception ex)
            {
                ErrorPopup.Show("LaunchFortniteCommand",
                    "An error occured. Please report this to the dev team.", ex);
            }
        });
    }
}