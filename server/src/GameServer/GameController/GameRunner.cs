using GameServer.GameLogic;
using Serilog;

namespace GameServer.GameController;

public class GameRunner : IGameRunner
{
    public Game Game { get; }

    public GameRunner(Config config, ILogger logger)
    {
        Game = new Game(config, logger);
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
