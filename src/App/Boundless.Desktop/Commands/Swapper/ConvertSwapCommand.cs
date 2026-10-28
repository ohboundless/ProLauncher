global using static Boundless.Swapper.Globals;
using System.Diagnostics;
using System.Windows.Input;
using Boundless.Desktop.Config;

namespace Boundless.Desktop.Commands.Swapper;

public class ConvertSwapCommand : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        if (parameter is not SwapParameter swap)
            throw new ApplicationException($"Parameter is not a SwapParameter ({parameter})");

        Task.Run(() =>
        {
            try
            {
                var templateId = swap.SwapTo.Cosmetic.TemplateId.Split(":");
                var athenaType = templateId[0];
                //var id = templateId[1];

                Log("ConvertSwapCommand", LogLevel.Info, $"Starting swap for {swap.SwapTo.Cosmetic.TemplateId}");
                
                swap.Log("Starting conversion...");
                var sw = Stopwatch.StartNew();

                switch (athenaType)
                {
                    case "AthenaCharacter":
                        CharacterSwap.Convert(swap);
                        break;
                    case "AthenaBackpack":
                        if (SwapperData.CharacterId == null)
                        {
                            swap.Log("Cannot swap backpack without first swapping a character!");
                            swap.Log("Cannot swap backpack without first swapping a character!");
                            swap.Log("Cannot swap backpack without first swapping a character!");
                        }
                        else
                            BackpackSwap.Convert(swap);
                        
                        break;
                    default:
                        swap.Log($"Swap type '{athenaType}' is not implemented yet!");
                        swap.Log($"Swap type '{athenaType}' is not implemented yet!");
                        swap.Log($"Swap type '{athenaType}' is not implemented yet!");
                        break;
                }

                swap.Log($"Converted in {sw.ElapsedMilliseconds}ms");
                
            }
            catch (Exception ex)
            {
                swap.Log(ex.ToString());
            }
        });
    }

    public event EventHandler? CanExecuteChanged;
}