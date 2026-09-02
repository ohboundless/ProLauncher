using System.Windows.Input;

namespace Boundless.Desktop.Commands;

public static class StaticCommands
{
    private static LaunchFortniteCommand _launchFortniteCommand;
    public static LaunchFortniteCommand LaunchFortniteCommand
    {
        get
        {
            if (_launchFortniteCommand != null)
                return _launchFortniteCommand;

            _launchFortniteCommand = new LaunchFortniteCommand();
            return _launchFortniteCommand;
        }
    }
    
    private static AddAccountCommand _addAccountCommand;
    public static AddAccountCommand AddAccountCommand
    {
        get
        {
            if (_addAccountCommand != null)
                return _addAccountCommand;

            _addAccountCommand = new AddAccountCommand();
            return _addAccountCommand;
        }
    }
}