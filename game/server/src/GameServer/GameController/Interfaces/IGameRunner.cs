using GameServer.GameLogic;

namespace GameServer.GameController;

public interface IGameRunner
{
    // Game to be controlled.
    public IGame Game { get; }

    // Start the game.
    public void Start();
    // Stop the game.
    public void Stop();
    // Reset the game.
    public void Reset();
}
