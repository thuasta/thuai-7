using System.Collections.Concurrent;

using GameServer.Connection;
using GameServer.GameLogic;
using GameServer.Geometry;

namespace GameServer.GameController;

public partial class GameRunner : IGameRunner
{

    private readonly ConcurrentDictionary<string, int> _tokenToPlayerId = new();
    private readonly ConcurrentDictionary<string, Guid> _tokenToSocketId = new();
    private int _nextPlayerId = 0;

    public void HandleAfterMessageReceiveEvent(object? sender, AfterMessageReceiveEventArgs e)
    {
        _logger.Debug($"Handling message: {e.Message.MessageType}");

        if (e.Message is not PerformMessage)
        {
            _logger.Error($"Message type {e.Message.MessageType} shouldn't come from a player.");
            return;
        }

        if (e.Message is GetPlayerInfoMessage getPlayerInfoMessage)
        {
            if (_tokenToSocketId.TryGetValue(getPlayerInfoMessage.Token, out Guid socketId) == true)
            {
                if (socketId != e.SocketId)
                {
                    _logger.Error(
                        $"Token \"{getPlayerInfoMessage.Token}\" is already used by another client."
                    );
                }
                else
                {
                    _logger.Warning(
                        $"Token \"{getPlayerInfoMessage.Token}\" is already used by the same client."
                    );
                    _logger.Warning(
                        $"The client can directly control player with token \"{getPlayerInfoMessage.Token}\"."
                    );
                }
                return;
            }
            else
            {
                if (_tokenToSocketId.Any(kvp => kvp.Value == e.SocketId))
                {
                    _logger.Error(
                        $"Client with socket id {e.SocketId} has already joined the game with token \"{getPlayerInfoMessage.Token}\"."
                    );
                    return;
                }

                _logger.Information(
                    $"Adding player {_nextPlayerId} with token \"{getPlayerInfoMessage.Token}\" to the game."
                );
                try
                {
                    Game.AddPlayer(
                        new Player(
                            _nextPlayerId,
                            Constant.PLAYER_MAXIMUM_HEALTH,
                            Constant.PLAYER_SPEED_PER_TICK,
                            new Position(0, 0)
                        )
                    );

                    _tokenToPlayerId[getPlayerInfoMessage.Token] = _nextPlayerId;
                    _tokenToSocketId[getPlayerInfoMessage.Token] = e.SocketId;

                    AfterNewPlayerJoinEvent?.Invoke(this, new AfterNewPlayerJoinEventArgs(
                        _nextPlayerId,
                        getPlayerInfoMessage.Token,
                        e.SocketId
                    ));

                    _nextPlayerId++;
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        $"Failed to add player with token \"{getPlayerInfoMessage.Token}\" to the game: {ex.Message}"
                    );
                }
            }
        }
        else
        {
            if (!_tokenToPlayerId.ContainsKey((e.Message as PerformMessage)!.Token))
            {
                _logger.Error($"Player with token \"{(e.Message as PerformMessage)!.Token}\" does not exist.");
                return;
            }
            if (_tokenToSocketId[(e.Message as PerformMessage)!.Token] != e.SocketId)
            {
                _logger.Error(
                    $"Player with token \"{(e.Message as PerformMessage)!.Token}\" is controlled by another client."
                );
                return;
            }

            switch (e.Message)
            {
                case PerformAbandonMessage performAbandonMessage:
                    try
                    {
                        (IItem.ItemKind, string) abandonedSupplies = new(
                            IItem.GetItemKind(performAbandonMessage.TargetSupply),
                            performAbandonMessage.TargetSupply
                        );

                        Game.AllPlayers.Find(p => p.PlayerId == _tokenToPlayerId[performAbandonMessage.Token])?
                        .PlayerAbandon(performAbandonMessage.Numb, abandonedSupplies);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"Failed to perform action \"Abandon\" for player with token {performAbandonMessage.Token}: {ex.Message}"
                        );
                    }
                    break;

                case PerformPickUpMessage performPickUpMessage:
                    try
                    {
                        Game.AllPlayers.Find(p => p.PlayerId == _tokenToPlayerId[performPickUpMessage.Token])?
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
                    break;

                case PerformSwitchArmMessage performSwitchArmMessage:
                    try
                    {
                        Game.AllPlayers.Find(p => p.PlayerId == _tokenToPlayerId[performSwitchArmMessage.Token])?
                        .PlayerSwitchArm(performSwitchArmMessage.TargetFirearm);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"Failed to perform action \"PickUp\" for player with token {performSwitchArmMessage.Token}: {ex.Message}"
                        );
                    }
                    break;

                case PerformUseMedicineMessage performUseMedicineMessage:
                    try
                    {
                        Game.AllPlayers.Find(p => p.PlayerId == _tokenToPlayerId[performUseMedicineMessage.Token])?
                        .PlayerUseMedicine(performUseMedicineMessage.MedicineName);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"Failed to perform action \"UseMedicine\" for player with token {performUseMedicineMessage.Token}: {ex.Message}"
                        );
                    }
                    break;

                case PerformUseGrenadeMessage performUseGrenadeMessage:
                    try
                    {
                        Game.AllPlayers.Find(p => p.PlayerId == _tokenToPlayerId[performUseGrenadeMessage.Token])?
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
                    break;

                case PerformMoveMessage performMoveMessage:
                    try
                    {
                        Game.AllPlayers.Find(p => p.PlayerId == _tokenToPlayerId[performMoveMessage.Token])?
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
                    break;

                case PerformStopMessage performStopMessage:
                    try
                    {
                        Game.AllPlayers.Find(p => p.PlayerId == _tokenToPlayerId[performStopMessage.Token])?
                        .Stop();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"Failed to perform action \"Stop\" for player with token {performStopMessage.Token}: {ex.Message}"
                        );
                    }
                    break;

                case PerformAttackMessage performAttackMessage:
                    try
                    {
                        Game.AllPlayers.Find(p => p.PlayerId == _tokenToPlayerId[performAttackMessage.Token])?
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
                    break;

                case GetMapMessage getMapMessage:
                    _logger.Error(
                        $"Message type {getMapMessage.MessageType} is no longer used. The server pulishes map information instead."
                    );
                    break;

                case ChooseOriginMessage chooseOriginMessage:
                    try
                    {
                        Game.AllPlayers.Find(p => p.PlayerId == _tokenToPlayerId[chooseOriginMessage.Token])?
                        .Teleport(
                            new Position(chooseOriginMessage.OriginPos.X, chooseOriginMessage.OriginPos.Y)
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"Failed to perform action \"ChooseOrigin\" for player with token {chooseOriginMessage.Token}: {ex.Message}"
                        );
                    }
                    break;

                default:
                    _logger.Error($"Unknown message type: {e.Message.MessageType}");
                    break;
            }
        }
    }
}
