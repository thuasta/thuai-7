using Serilog;

namespace GameServer.GameLogic;

public partial class Game
{
    private readonly ILogger _logger;
    /// <summary>
    /// The default time gap between ticks.
    /// </summary>
    /// <remarks>
    /// This is not the TPS of the game. On the contrary, this is the time gap between ticks,
    /// which is the "real" duration between two ticks for physics simulation and other
    /// calculations.
    /// </remarks>
    private const decimal DefaultTimeGap = 0.05m;

    /// <summary>
    /// The interval of TPS check in seconds.
    /// </summary>
    private const decimal TpsCheckInterval = 10.0m;


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

    public decimal TicksPerSecond { get; private set; }

    private DateTime _lastTpsCheckTime = DateTime.Now;
    private DateTime? _lastTickTime = null; // In microseconds.

    #endregion


    #region Constructors and finalizers
    /// <summary>
    /// Initializes a new instance of the <see cref="Game"/> class.
    /// </summary>
    public Game(Config config, ILogger logger)
    {
        _logger = logger;
        Config = config;
        TicksPerSecond = config.TicksPerSecond;

        _map = new Map(config.MapWidth, config.MapHeight);
        _allPlayers = new List<Player>();


    }

    #endregion


    #region Methods
    /// <summary>
    /// Runs the game.
    /// </summary>
    /// <remarks>
    /// Note that this is not used to resume the game after it has been paused.
    /// </remarks>
    public void Run()
    {
        if (_lastTickTime is not null)
        {
            throw new InvalidOperationException("The game is already running.");
        }

        _lastTickTime = DateTime.Now;
        // Clear the map.
        _map.Clear();
        // Regenerate the map.
        _map.GenerateMap();
        _logger.Information("The game is running...");

    }

    /// <summary>
    /// Stops the game.
    /// </summary>
    /// <remarks>
    /// Note that this is not used to pause the game.
    /// </remarks>
    public void Stop()
    {
        if (_lastTickTime is null)
        {
            _logger.Warning("The game is already stopped.");
        }

        lock (this)
        {
            _lastTickTime = null;
        }

    }

    /// <summary>
    /// Ticks the game. This method is called every tick to update the game.
    /// </summary>
    public void Tick()
    {
        try
        {
            lock (this)
            {
                if (_lastTickTime is null)
                {
                    return;
                }

                DateTime currentTime = DateTime.Now;

                if (currentTime - _lastTickTime < TimeSpan.FromSeconds((double)(1.0m / Config.TicksPerSecond)))
                {
                    return;
                }

                TicksPerSecond = 1.0m / (decimal)(currentTime - _lastTickTime.Value).TotalSeconds;
                _lastTickTime = currentTime;

                // Check TPS.
                if (DateTime.Now - _lastTpsCheckTime > TimeSpan.FromSeconds((double)TpsCheckInterval))
                {
                    _lastTpsCheckTime = DateTime.Now;
                    if (TicksPerSecond < Config.TicksPerSecond * 0.9m)
                    {
                        _logger.Warning($"Insufficient simulation rate: {TicksPerSecond:0.00} tps < {Config.TicksPerSecond} tps");
                    }
                }
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
