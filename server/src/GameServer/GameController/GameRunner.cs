using GameServer.GameLogic;
using Serilog;

namespace GameServer.GameController;

public class GameRunner : IGameRunner
{
    public Game Game { get; }
    public int ExpectedTicksPerSecond => Constant.TICKS_PER_SECOND;
    public TimeSpan TpsCheckInterval => TimeSpan.FromSeconds(10);
    public double RealTicksPerSecond { get; private set; }
    public double TpsLowerBound => 0.9 * ExpectedTicksPerSecond;
    public double TpsUpperBound => 1.1 * ExpectedTicksPerSecond;

    private DateTime _lastTpsCheckTime = DateTime.Now;

    private Task? _tickTask = null;
    private bool _isRunning = false;

    private readonly ILogger _logger = Log.ForContext("Component", "GameRunner");

    public GameRunner(Config config, ILogger logger)
    {
        Game = new Game(config);
        _logger = logger;
    }

    public void Start()
    {
        _logger.Information("Starting game server...");

        Game.Initialize();

        _tickTask = new Task(() =>
        {
            DateTime lastTickTime = DateTime.Now;

            while (_isRunning)
            {
                Game.Tick();
                Task.Delay(1000 / ExpectedTicksPerSecond).Wait();

                DateTime currentTime = DateTime.Now;
                RealTicksPerSecond = 1.0D / (double)(currentTime - lastTickTime).TotalSeconds;
                lastTickTime = currentTime;

                // Check TPS.
                if (DateTime.Now - _lastTpsCheckTime > TpsCheckInterval)
                {
                    _lastTpsCheckTime = DateTime.Now;
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

    }

    public void Stop()
    {
        _isRunning = false;

        _tickTask?.Wait();
        _tickTask?.Dispose();
        _tickTask = null;
    }

    public void Reset()
    {
        // TODO: Implement
        throw new NotImplementedException();
    }
}
