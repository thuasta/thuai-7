using Serilog;

namespace GameServer.GameLogic;

public partial class Game
{
    public event EventHandler<AfterGameTickEventArgs>? AfterGameTickEvent = delegate { };

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

    private readonly object _lock = new();

    #endregion

    #region Constructors and finalizers
    /// <summary>
    /// Initializes a new instance of the <see cref="Game"/> class.
    /// </summary>
    public Game(Config config)
    {
        _logger = Log.ForContext("Component", "Game");
        Config = config;

        GameMap = new Map(config.MapWidth, config.MapHeight, config.SafeZoneMaxRadius, config.SafeZoneTicksUntilDisappear, config.DamageOutsideSafeZone);
        AllPlayers = new List<Player>();

    }

    #endregion


    #region Methods
    /// <summary>
    /// Initializes the game.
    /// </summary>
    public void Initialize()
    {
        SubscribePlayerEvents();
        GameMap.GenerateMap();
    }

    /// <summary>
    /// Ticks the game. This method is called every tick to update the game.
    /// </summary>
    public void Tick()
    {
        try
        {
            lock (_lock)
            {
                CurrentTick++;

                UpdateMap();
                UpdatePlayers();
                UpdateGrenades();

                AfterGameTickEvent?.Invoke(this, new AfterGameTickEventArgs(this, CurrentTick));
            }

        }
        catch (Exception e)
        {
            _logger.Error($"An exception occurred while ticking the game: {e}");
        }
    }
    # endregion
}
