using System.Windows.Input;
using Boundless.Desktop.Commands.Swapper;

namespace Boundless.Desktop.Commands;

public static class StaticCommands
{
    #region Account
    
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
    
    private static RemoveAccountCommand _removeAccountCommand;
    public static RemoveAccountCommand RemoveAccountCommand
    {
        get
        {
            if (_removeAccountCommand != null)
                return _removeAccountCommand;

            _removeAccountCommand = new RemoveAccountCommand();
            return _removeAccountCommand;
        }
    }
    
    private static RefreshProfileCommand _refreshProfileCommand;
    public static RefreshProfileCommand RefreshProfileCommand
    {
        get
        {
            if (_refreshProfileCommand != null)
                return _refreshProfileCommand;

            _refreshProfileCommand = new RefreshProfileCommand();
            return _refreshProfileCommand;
        }
    }
    
    #endregion

    #region Swapper

    private static ConvertSwapCommand _convertSwapCommand;
    public static ConvertSwapCommand ConvertSwapCommand
    {
        get
        {
            if (_convertSwapCommand != null)
                return _convertSwapCommand;

            _convertSwapCommand = new ConvertSwapCommand();
            return _convertSwapCommand;
        }
    }

    private static RevertSwapCommand _revertSwapCommand;
    public static RevertSwapCommand RevertSwapCommand
    {
        get
        {
            if (_revertSwapCommand != null)
                return _revertSwapCommand;

            _revertSwapCommand = new RevertSwapCommand();
            return _revertSwapCommand;
        }
    }
    
    #endregion
}