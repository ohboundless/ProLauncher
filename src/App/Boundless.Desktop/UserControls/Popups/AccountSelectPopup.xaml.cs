using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Boundless.Desktop.Commands.Swapper;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.Utils;
using Boundless.Desktop.ViewModels;

namespace Boundless.Desktop.UserControls.Popups;

public partial class AccountSelectPopup : UserControl
{
    private Action<FortniteAccount> _selectedAction;
    private IEnumerable<FortniteAccount> _accounts;
    private bool _isClosing;
    
    private string _selectedAccount;
    public string SelectedAccount => _selectedAccount;
    
    public AccountSelectPopup(IEnumerable<FortniteAccount> accounts, Action<FortniteAccount> selectedAction)
    {
        _accounts = accounts;
        _selectedAction = selectedAction;
        InitializeComponent();

        foreach (var account in accounts)
        {
            var item = new ComboBoxItem
            {
                Content = new TextBlock { Text = account.Username, Foreground = Brushes.White, ToolTip = account.Id }
            };

            item.Selected += AccountSelected;
            Accounts.Items.Add(item);
        }
        
        MainWindow.PopupContainer.AddPopup(this);
    }

    private void AccountSelected(object sender, RoutedEventArgs e)
    {
        var item = (ComboBoxItem)sender;
        var textBlock = (TextBlock)item.Content;

        _selectedAccount = (string)textBlock.ToolTip;
    }

    private void Close_OnClick(object sender, RoutedEventArgs e)
    {
        MainWindow.PopupContainer.RemovePopup(this);
        Close();
    }

    private void Confirm_OnClick(object sender, RoutedEventArgs e)
    {
        _selectedAction(_accounts.First(x => x.Id == SelectedAccount));
        Close();
    }

    private void Close()
    {
        _isClosing = true;
        MainWindow.PopupContainer.RemovePopup(this);
    }
}