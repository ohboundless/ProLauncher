using Boundless.Desktop.UserControls.Popups;
using Boundless.Desktop.ViewModels;

namespace Boundless.Desktop.Commands.Swapper;

public class SwapParameter(SwapControlPopup? control, CosmeticItemModel? swapFrom, CosmeticItemModel swapTo)
{
    public SwapControlPopup? Control = control;
    public CosmeticItemModel? SwapFrom = swapFrom;
    public CosmeticItemModel SwapTo = swapTo;

    public Action<string> Log
    {
        get
        {
            if (Control == null)
                return _ => { };

            return Control.LogMessage;
        }
    }
}