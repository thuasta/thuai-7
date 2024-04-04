using Serilog;

namespace GameServer.GameLogic;

public partial class Game
{
    #region Fields and properties
    /// <summary>
    /// Gets the config of the game.
    /// </summary>
    public Config Config { get; }
    /// <summary>
    /// Gets the current tick of the game.
    /// </summary>
    /// <remarks>
    /// The first tick is 0.
    /// </remarks>
    public int CurrentTick { get; private set; } = 0;

    private readonly ILogger _logger;

    #endregion


    #region Constructors and finalizers
    /// <summary>
    /// Initializes a new instance of the <see cref="Game"/> class.
    /// </summary>
    public Game(Config config)
    {
        _logger = Log.ForContext("Component", "Game");
        Config = config;

        _map = new Map(config.MapWidth, config.MapHeight);
        _allPlayers = new List<Player>();


    }

    #endregion


    #region Methods

    /// <summary>
    /// Ticks the game. This method is called every tick to update the game.
    /// </summary>
    public void Tick()
    {
        try
        {
            lock (this)
            {
                UpdateObjects();
                // UpdateCircle();
                UpdatePlayers();
                // UpdateBullets();
                UpdateGrenades();

                // AfterGameTickEvent?.Invoke(this, new AfterGameTickEventArgs(this, CurrentTick));

                // Accumulate the current tick at the end of the tick.
                CurrentTick++;
            }

        }
        catch (Exception e)
        {
            _logger.Error($"An exception occurred while ticking the game: {e}");
        }

    }

    private void UpdateObjects()
    {
        // The object will be deleted if it is picked up by the player.
        throw new NotImplementedException();
    }

    # endregion
}
