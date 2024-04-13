namespace GameServer.GameLogic;

public class AfterGameTickEventArgs : EventArgs
{
    public Game Game { get; }
    public int CurrentTick { get; }

    public AfterGameTickEventArgs(Game game, int currentTick)
    {
        Game = game;
        CurrentTick = currentTick;
    }
}
