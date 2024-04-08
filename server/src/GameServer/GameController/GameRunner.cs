using System.Collections.Concurrent;
using GameServer.Connection;
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

    private readonly ConcurrentDictionary<string, int> _tokenToPlayerId = new();
    private int _nextPlayerId = 0;

    private DateTime _lastTpsCheckTime = DateTime.Now;

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
            DateTime lastTickTime = DateTime.Now;

            while (_isRunning)
            {
                Game.Tick();

                while (DateTime.Now - lastTickTime < TimeSpan.FromMilliseconds(1000 / ExpectedTicksPerSecond))
                {
                    // Wait for the next tick
                }

                DateTime currentTime = DateTime.Now;
                RealTicksPerSecond = 1.0D / (double)(currentTime - lastTickTime).TotalSeconds;
                lastTickTime = currentTime;

                // Check TPS.
                if (DateTime.Now - _lastTpsCheckTime > TpsCheckInterval)
                {
                    _lastTpsCheckTime = DateTime.Now;

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

    public void Stop()
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
    }

    public void Reset()
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    public void HandleAfterMessageReceiveEvent(object? sender, AfterMessageReceiveEventArgs e)
    {
        switch (e.Message)
        {
            case PerformAbandonMessage performAbandonMessage:
                if (!_tokenToPlayerId.ContainsKey(performAbandonMessage.Token))
                {
                    _logger.Error($"Player with token {performAbandonMessage.Token} does not exist.");
                }
                else
                {
                    try
                    {
                        List<(IItem.ItemKind, string)> abandonedSupplies = new()
                        {
                            (IItem.GetItemKind(performAbandonMessage.TargetSupply), performAbandonMessage.TargetSupply)
                        };

                        Game.AllPlayers.Find(p => p.Id == _tokenToPlayerId[performAbandonMessage.Token])?
                        .PlayerAbandon(performAbandonMessage.Numb, abandonedSupplies);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"Failed to perform action \"Abandon\" for player with token {performAbandonMessage.Token}: {ex.Message}"
                        );
                    }
                }
                break;
            case PerformPickUpMessage performPickUpMessage:
                break;
            case PerformSwitchArmMessage performSwitchArmMessage:
                break;
            case PerformUseMedicineMessage performUseMedicineMessage:
                break;
            case PerformUseGrenadeMessage performUseGrenadeMessage:
                break;
            case PerformMoveMessage performMoveMessage:
                break;
            case PerformStopMessage performStopMessage:
                break;
            case PerformAttackMessage performAttackMessage:
                break;
            case GetPlayerInfoMessage getPlayerInfoMessage:
                break;
            case GetMapMessage getMapMessage:
                break;
            case ChooseOriginMessage chooseOriginMessage:
                if (!_tokenToPlayerId.ContainsKey(chooseOriginMessage.Token))
                {
                    _logger.Information($"Adding player with token {chooseOriginMessage.Token} to the game.");
                    try
                    {
                        Game.AddPlayer(
                            new Player(
                                _nextPlayerId,
                                Constant.PLAYER_MAXIMUM_HEALTH,
                                Constant.PLAYER_SPEED_PER_TICK,
                                new Position(chooseOriginMessage.OriginPos.X, chooseOriginMessage.OriginPos.Y)
                            )
                        );
                        _tokenToPlayerId[chooseOriginMessage.Token] = _nextPlayerId;
                        _nextPlayerId++;

                        _logger.Information($"Player with token {chooseOriginMessage.Token} added to the game.");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"Failed to add player with token {chooseOriginMessage.Token} to the game: {ex.Message}"
                        );
                    }
                }
                else
                {
                    _logger.Error($"Player with token {chooseOriginMessage.Token} already exists.");
                }
                break;

            default:
                _logger.Error($"Unknown message type: {e.Message.MessageType}");
                break;
        }
    }
}
