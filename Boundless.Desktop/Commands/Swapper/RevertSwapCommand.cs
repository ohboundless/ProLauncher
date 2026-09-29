using System.Windows.Input;

namespace Boundless.Desktop.Commands.Swapper;

public class RevertSwapCommand : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        
    }

    public event EventHandler? CanExecuteChanged;
}