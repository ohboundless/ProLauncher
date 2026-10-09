global using static Boundless.Logging.Classes.Logging;
global using static Boundless.Swapper.Globals;
using Boundless.Swapper.Core;

namespace Boundless.Swapper;

public static class Globals
{
    public static SwapperCore SC;
    public static UnrealSystemProvider USystem => SC.USystem;
}