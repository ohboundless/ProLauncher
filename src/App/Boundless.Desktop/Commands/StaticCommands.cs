using System.Windows.Input;
using Boundless.Desktop.Commands.Swapper;

namespace Boundless.Desktop.Commands;

public static class StaticCommands
{
    #region Account

    public static LaunchFortniteCommand LaunchFortniteCommand
    {
        get
        {
            if (field != null)
                return field;

            field = new LaunchFortniteCommand();
            return field;
        }
    }

    public static AddAccountCommand AddAccountCommand
    {
        get
        {
            if (field != null)
                return field;

            field = new AddAccountCommand();
            return field;
        }
    }

    public static RemoveAccountCommand RemoveAccountCommand
    {
        get
        {
            if (field != null)
                return field;

            field = new RemoveAccountCommand();
            return field;
        }
    }

    public static RefreshProfileCommand RefreshProfileCommand
    {
        get
        {
            if (field != null)
                return field;

            field = new RefreshProfileCommand();
            return field;
        }
    }
    
    #endregion

    #region Swapper

    public static ConvertSwapCommand ConvertSwapCommand
    {
        get
        {
            if (field != null)
                return field;

            field = new ConvertSwapCommand();
            return field;
        }
    }

    public static RevertSwapCommand RevertSwapCommand
    {
        get
        {
            if (field != null)
                return field;

            field = new RevertSwapCommand();
            return field;
        }
    }
    
    #endregion
}