namespace GameServer.GameLogic;

public class AfterGameTickEventArgs : EventArgs
{
    public List<Player> AllPlayers { get; }
    public Map GameMap { get; }
    public int CurrentTick { get; }

    public AfterGameTickEventArgs(List<Player> allPlayers, Map gameMap, int currentTick)
    {
        AllPlayers = new(allPlayers);
        GameMap = gameMap;
        CurrentTick = currentTick;
    }
}
