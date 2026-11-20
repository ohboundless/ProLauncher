using System.Diagnostics;
using System.Windows;

namespace Boundless.Installer.Windows;

public partial class MessagePopup : Window
{
    public MessagePopup(string title, string message)
    {
        InitializeComponent();
        
        TitleBlock.Text = title;
        Message.Text = message;
    }

    private void Ok_OnClick(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(Message.Text);
        
        Close();
        Thread.Sleep(5000);
        Process.GetCurrentProcess().Kill();
    }
}