using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Boundless.Desktop.Games.EpicGames.Fortnite;
using Boundless.Desktop.Utils;
using Boundless.Desktop.Windows.Fortnite;

namespace Boundless.Desktop.UserControls.Tabs.Games.Fortnite;

public partial class FortniteTab : TabWithBack
{
    public FortniteTab(Action backAction) : base(backAction)
    {
        InitializeComponent();
    }

    private bool IsLoggingIn = false;
    private async Task TriggerLogin()
    {
        if (IsLoggingIn)
        {
            Log(ELogCategory.UI, ELogLevel.Info, "Fortnite login already initialized, returning...");
            return;
        }
        
        Log(ELogCategory.UI, ELogLevel.Info, "Fortnite login triggered...");
        
        IsLoggingIn = true;
        LoginBtn.Content = "Loading...";
        await FortniteHandler.StartAuthorization();

        Log(ELogCategory.UI, ELogLevel.Info, "Fortnite login completed, making tab visible...");
        Login.Visibility = Visibility.Collapsed;
        Main.Visibility = Visibility.Visible;
        LoginBtn.Content = "Login";
    }

    private void BackArrow_MouseUp(object sender, MouseButtonEventArgs e)
    {
        BackAction();
    }

    private void AccountCheckerBtn_OnClick(object sender, RoutedEventArgs e)
    {
        new FortniteLockerWindow().Show();
    }

    private async void Login_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            await TriggerLogin();
        }
        catch (TimeoutException)
        {
            // TODO handle
        }
        catch (Exception ex)
        {
            // TODO handle
        }
        finally
        {
            if (FortniteAccount.Recent != null)
                LoadAccount(FortniteAccount.Recent);
        }
    }

    public void LoadAccount(FortniteAccount account)
    {
        AccountName.Text = account.Username;
        AccountIcon.Source = new BitmapImage(new Uri(account.Icon));
    }
}