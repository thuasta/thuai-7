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
        if (_isRunning == false)
        {
            _logger.Warning($"Game is not running. Ignoring message: {e.Message.MessageType}");
            return;
        }

        _logger.Debug($"Handling message: {e.Message.MessageType}");

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
                if (!_tokenToPlayerId.ContainsKey(performPickUpMessage.Token))
                {
                    _logger.Error($"Player with token {performPickUpMessage.Token} does not exist.");
                }
                else
                {
                    try
                    {
                        Game.AllPlayers.Find(p => p.Id == _tokenToPlayerId[performPickUpMessage.Token])?
                        .PlayerPickUp(
                            performPickUpMessage.TargetSupply,
                            new Position(performPickUpMessage.TargetPos.X, performPickUpMessage.TargetPos.Y),
                            performPickUpMessage.Num
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"Failed to perform action \"PickUp\" for player with token {performPickUpMessage.Token}: {ex.Message}"
                        );
                    }
                }
                break;

            case PerformSwitchArmMessage performSwitchArmMessage:
                if (!_tokenToPlayerId.ContainsKey(performSwitchArmMessage.Token))
                {
                    _logger.Error($"Player with token {performSwitchArmMessage.Token} does not exist.");
                }
                else
                {
                    try
                    {
                        Game.AllPlayers.Find(p => p.Id == _tokenToPlayerId[performSwitchArmMessage.Token])?
                        .PlayerSwitchArm(performSwitchArmMessage.TargetFirearm);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"Failed to perform action \"PickUp\" for player with token {performSwitchArmMessage.Token}: {ex.Message}"
                        );
                    }
                }
                break;

            case PerformUseMedicineMessage performUseMedicineMessage:
                if (!_tokenToPlayerId.ContainsKey(performUseMedicineMessage.Token))
                {
                    _logger.Error($"Player with token {performUseMedicineMessage.Token} does not exist.");
                }
                else
                {
                    try
                    {
                        Game.AllPlayers.Find(p => p.Id == _tokenToPlayerId[performUseMedicineMessage.Token])?
                        .PlayerUseMedicine(performUseMedicineMessage.MedicineName);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"Failed to perform action \"UseMedicine\" for player with token {performUseMedicineMessage.Token}: {ex.Message}"
                        );
                    }
                }
                break;

            case PerformUseGrenadeMessage performUseGrenadeMessage:
                if (!_tokenToPlayerId.ContainsKey(performUseGrenadeMessage.Token))
                {
                    _logger.Error($"Player with token {performUseGrenadeMessage.Token} does not exist.");
                }
                else
                {
                    try
                    {
                        Game.AllPlayers.Find(p => p.Id == _tokenToPlayerId[performUseGrenadeMessage.Token])?
                        .PlayerUseGrenade(
                            new Position(performUseGrenadeMessage.TargetPos.X, performUseGrenadeMessage.TargetPos.Y)
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"Failed to perform action \"UseGrenade\" for player with token {performUseGrenadeMessage.Token}: {ex.Message}"
                        );
                    }
                }
                break;

            case PerformMoveMessage performMoveMessage:
                if (!_tokenToPlayerId.ContainsKey(performMoveMessage.Token))
                {
                    _logger.Error($"Player with token {performMoveMessage.Token} does not exist.");
                }
                else
                {
                    try
                    {
                        Game.AllPlayers.Find(p => p.Id == _tokenToPlayerId[performMoveMessage.Token])?
                        .MoveTo(
                            new Position(performMoveMessage.Destination.X, performMoveMessage.Destination.Y)
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"Failed to perform action \"Move\" for player with token {performMoveMessage.Token}: {ex.Message}"
                        );
                    }
                }
                break;

            case PerformStopMessage performStopMessage:
                if (!_tokenToPlayerId.ContainsKey(performStopMessage.Token))
                {
                    _logger.Error($"Player with token {performStopMessage.Token} does not exist.");
                }
                else
                {
                    try
                    {
                        Game.AllPlayers.Find(p => p.Id == _tokenToPlayerId[performStopMessage.Token])?
                        .Stop();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"Failed to perform action \"Stop\" for player with token {performStopMessage.Token}: {ex.Message}"
                        );
                    }
                }
                break;

            case PerformAttackMessage performAttackMessage:
                if (!_tokenToPlayerId.ContainsKey(performAttackMessage.Token))
                {
                    _logger.Error($"Player with token {performAttackMessage.Token} does not exist.");
                }
                else
                {
                    try
                    {
                        Game.AllPlayers.Find(p => p.Id == _tokenToPlayerId[performAttackMessage.Token])?
                        .PlayerAttack(
                            new Position(performAttackMessage.TargetPos.X, performAttackMessage.TargetPos.Y)
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"Failed to perform action \"Attack\" for player with token {performAttackMessage.Token}: {ex.Message}"
                        );
                    }
                }
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
