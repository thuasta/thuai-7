using GameServer.GameLogic;

namespace GameServer.GameController;

public class GameRunner : IGameRunner
{
    public Game Game { get; }

    public GameRunner(Config config)
    {
        Game = new Game(config);
    }
    public void Start()
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    public void Stop()
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    public void Reset()
    {
        // TODO: Implement
        throw new NotImplementedException();
    }
}
