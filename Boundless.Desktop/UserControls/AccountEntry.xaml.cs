using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Boundless.Desktop.Games.EpicGames.Fortnite;

namespace Boundless.Desktop.UserControls;

public partial class AccountEntry : UserControl
{
    public FortniteAccount Account;
    
    public AccountEntry(FortniteAccount account)
    {
        Account = account;
        InitializeComponent();
        Populate();
    }

    private void Populate()
    {
        AccountName.Text = Account.Username;
        AccountIcon.Source = new BitmapImage(new Uri(Account.Icon));
        LastPlayTime.Text = $"Last launched {new Random().Next(1, 365)}d ago";
    }
}