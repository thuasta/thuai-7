namespace GameServer.GameLogic;

public class AfterGameInitializationEventArgs : EventArgs
{
    public Game Game { get; }

    public AfterGameInitializationEventArgs(Game game)
    {
        Game = game;
    }
}
