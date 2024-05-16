using GameServer.GameLogic;
using Serilog;

namespace GameServer.GameController;

public partial class GameRunner
{
    public event EventHandler<AfterPlayerConnect>? AfterPlayerConnectEvent = delegate { };

    public Game Game { get; }

    public bool WhiteListMode { get; init; } = false;
    public List<string> WhiteList { get; init; } = new();

    public int ExpectedTicksPerSecond => Constant.TICKS_PER_SECOND;
    public TimeSpan TpsCheckInterval => TimeSpan.FromSeconds(10);
    public double RealTicksPerSecond { get; private set; }
    public double TpsLowerBound => 0.9 * ExpectedTicksPerSecond;
    public double TpsUpperBound => 1.1 * ExpectedTicksPerSecond;

    private const double WATING_TIME_RATIO = 0.8;

    private DateTime _lastTpsCheckTime = DateTime.UtcNow;

    private Task? _tickTask = null;
    private bool _isRunning = false;

    private readonly ILogger _logger = Log.ForContext("Component", "GameRunner");

    public GameRunner(Config config)
    {
        Game = new Game(config);
    }

    public void Start()
    {
        _logger.Information("Starting game...");

        Game.Initialize();

        _tickTask = new Task(() =>
        {
            DateTime lastTickTime = DateTime.UtcNow;

            while (_isRunning)
            {
                DateTime expectedNextTickTime
                    = lastTickTime + TimeSpan.FromMilliseconds(1000 / ExpectedTicksPerSecond);

                Game.Tick();

                if (DateTime.UtcNow < expectedNextTickTime)
                {
                    Task.Delay(
                        (int)((expectedNextTickTime - DateTime.UtcNow).TotalMilliseconds * WATING_TIME_RATIO)
                    ).Wait();
                }

                while (DateTime.UtcNow < expectedNextTickTime)
                {
                    // Wait for the next tick
                }

                DateTime currentTime = DateTime.UtcNow;
                RealTicksPerSecond = 1.0D / (double)(currentTime - lastTickTime).TotalSeconds;
                lastTickTime = currentTime;

                // Check TPS.
                if (DateTime.UtcNow - _lastTpsCheckTime > TpsCheckInterval)
                {
                    _lastTpsCheckTime = DateTime.UtcNow;

                    _logger.Debug($"Current TPS: {RealTicksPerSecond:0.00} tps");

                    if (RealTicksPerSecond < TpsLowerBound)
                    {
                        _logger.Warning($"Insufficient simulation rate: {RealTicksPerSecond:0.00} tps < {TpsLowerBound} tps");
                    }
                    if (RealTicksPerSecond > TpsUpperBound)
                    {
                        _logger.Warning($"Excessive simulation rate: {RealTicksPerSecond:0.00} tps > {TpsUpperBound} tps");
                    }
                }
            }
        });

        _isRunning = true;

        _tickTask.Start();

        _logger.Information("Game started.");

    }

    public void Stop(bool forceStop = false)
    {
        try
        {
            _isRunning = false;
            _logger.Information("Server stop requested.");

            // Stop the game.
            _logger.Information("Stopping server...");
            _tickTask?.Wait();
            _tickTask?.Dispose();
            _tickTask = null;

            // Save records.
            _logger.Information("Saving records...");
            Game.SaveRecord();

            if (forceStop == false)
            {
                int winnerId = Game.Judge();
                Result result = new()
                {
                    Winner = _tokenToPlayerId.First(kvp => kvp.Value == winnerId).Key,
                    WinnerPlayerId = winnerId
                };
                Game.SaveResults(result);
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to stop game: {ex.Message}");
            _logger.Debug($"{ex}");
        }
    }

    public void Reset()
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

}
