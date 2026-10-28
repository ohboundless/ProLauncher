using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Boundless.Desktop.Config;
using Boundless.Desktop.Games.EpicGames.Fortnite;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;

namespace Boundless.Desktop.UserControls;

public partial class AccountEntry : UserControl
{
    public FortniteAccount Account;
    private StackPanel _panel;

    private bool _isActive;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (value)
            {
                Container.Background = new SolidColorBrush(Color.FromRgb(58, 67, 119));
                Container.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 59, 106));
            }
            else
            {
                Container.Background = new SolidColorBrush(Colors.Transparent);
                Container.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }

            _isActive = value;
        }
    }
    
    public AccountEntry(FortniteAccount account, StackPanel panel)
    {
        InitializeComponent();
        IsActive = false;
        _panel = panel;
        
        Account = account;
        Populate();
    }

    private void Populate()
    {
        AccountName.Text = Account.Username;
        AccountIcon.Source = new BitmapImage(new Uri(Account.Icon));
        LastPlayTime.Text = BuildLastLaunchedString(Account.LastLaunchTime);
    }

    private string BuildLastLaunchedString(DateTime? dateTime)
    {
        if (!dateTime.HasValue)
        {
            return "Never launched";
        }

        var result = "Last launched ";
        var now = DateTime.Now;
        var timeSince = now.Subtract(dateTime.Value);
        
        if (TimeSpan.FromMinutes(1) > timeSince) // Less than a minute
        {
            result += $"{timeSince.Seconds}s ago";
        }
        else if (TimeSpan.FromHours(1) > timeSince) // Less than a hour
        {
            result += $"{timeSince.Minutes} min ago";
        }
        else if (TimeSpan.FromDays(1) > timeSince) // Less than a day
        {
            result += $"{timeSince.Hours}h ago";
        }
        else if (TimeSpan.FromDays(365) > timeSince) // Less than a year
        {
            result += $"{timeSince.Days}d ago";
        }
        else // More than a year
        {
            result += $"{timeSince.Days / 365} year(s) ago";
        }

        return result;
    }

    private async void AccountEntry_OnClick(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Right)
            return;
        
        foreach (var child in _panel.Children)
        {
            if (child is AccountEntry entry)
                entry.IsActive = false;
        }

        IsActive = true;
        ConfigData.CurrentAccounts.AccountIdsByGame["Fortnite"] = Account.Id;
        ConfigData.Save();

        // Callback
        await MainWindow.OnSetActiveAccount(Account);
    }

    private void MoreButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button { ContextMenu: not null } button)
        {
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.IsOpen = true;
        }
    }
}