using System.Diagnostics;
using System.Windows.Input;
using Boundless.Desktop.Config;
using Boundless.Swapper.Swappers;

namespace Boundless.Desktop.Commands.Swapper;

public class RevertSwapCommand : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        if (parameter is not SwapParameter swap)
            throw new ApplicationException($"Parameter is not a SwapParameter ({parameter})");
        
        var templateId = swap.SwapTo.Cosmetic.TemplateId.Split(":");
        var athenaType = templateId[0];

        Log("RevertSwapCommand", LogLevel.Info, $"Reverting swap for {swap.SwapTo.Cosmetic.TemplateId}");
        
        swap.Log("Reverting...");
        var sw = Stopwatch.StartNew();
        
        switch (athenaType)
        {
            case "AthenaCharacter":
                CharacterSwap.Revert(swap);
                break;
            case "AthenaBackpack":
                BackpackSwap.Revert(swap);
                break;
        }
        
        swap.Log($"Reverted in {sw.ElapsedMilliseconds}ms");
    }

    public event EventHandler? CanExecuteChanged;
}