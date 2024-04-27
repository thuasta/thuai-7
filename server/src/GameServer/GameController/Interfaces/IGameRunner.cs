using GameServer.GameLogic;

namespace GameServer.GameController;

public interface IGameRunner
{
    public event EventHandler<AfterPlayerConnect>? AfterPlayerConnectEvent;

    /// <summary>
    /// Game to be controlled.
    /// </summary>
    public Game Game { get; }

    /// <summary>
    /// The expected ticks per second.
    /// </summary>
    public int ExpectedTicksPerSecond { get; }

    /// <summary>
    /// Real ticks per second.
    /// </summary>
    public double RealTicksPerSecond { get; }

    /// <summary>
    /// The lower bound of allowed TPS.
    /// </summary>
    public double TpsLowerBound { get; }

    /// <summary>
    /// The upper bound of allowed TPS.
    /// </summary>
    public double TpsUpperBound { get; }

    /// <summary>
    /// The interval of TPS check.
    /// </summary>
    public TimeSpan TpsCheckInterval { get; }

    // Start the game.
    public void Start();
    // Stop the game.
    public void Stop();
    // Reset the game.
    public void Reset();

    /// <summary>
    /// Handle AfterMessageReceiveEvent of AgentServer.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void HandleAfterMessageReceiveEvent(object? sender, Connection.AfterMessageReceiveEventArgs e);
}
