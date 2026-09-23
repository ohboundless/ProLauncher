global using static Boundless.Swapper.Globals;
using Boundless.Swapper.Core;

namespace Boundless.Swapper;

public static class Globals
{
    public static SwapperCore SC { get; set; }
    public static UnrealSystemProvider USystem => SC.USystem;
}