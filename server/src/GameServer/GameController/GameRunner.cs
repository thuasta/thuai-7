using GameServer.GameLogic;

namespace GameServer.GameController;

public class GameRunner : IGameRunner
{
    public IGame Game { get; } = new Game();

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
