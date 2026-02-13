using System.Diagnostics;

namespace Boundless.Desktop.Games;

public class LaunchedGame(string name, Process? process)
{
    public string Name { get; } = name;
    public Process? Process { get; } = process;
}

public static class GameList
{
    public static List<LaunchedGame> LaunchedGames { get; } = [];
}