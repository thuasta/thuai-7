using System.Collections.Concurrent;

using GameServer.Connection;
using GameServer.GameLogic;
using GameServer.Geometry;

namespace GameServer.GameController;

public partial class GameRunner
{

    private readonly ConcurrentDictionary<string, int> _tokenToPlayerId = new();
    private int _nextPlayerId = 0;

    public void HandleAfterMessageReceiveEvent(object? sender, AfterMessageReceiveEventArgs e)
    {
        _logger.Debug($"Handling message: \"{e.Message.MessageType}\"");

        if (e.Message is not PerformMessage)
        {
            _logger.Error($"Message type {e.Message.MessageType} shouldn't come from a player.");
            return;
        }

        string playerToken = (e.Message as PerformMessage)!.Token;
        string playerTokenForLogging = playerToken.Length > 16 ? string.Concat(playerToken.AsSpan(0, 16), "...") : playerToken;

        if ((WhiteListMode == true) && (WhiteList.Any(token => token == playerToken) == false))
        {
            _logger.Error($"Token {playerTokenForLogging} is not in the whitelist.");
            return;
        }

        if (e.Message is GetPlayerInfoMessage getPlayerInfoMessage)
        {
            if (_tokenToPlayerId.Any(kvp => kvp.Key == getPlayerInfoMessage.Token) == true)
            {
                _logger.Debug(
                    $"Client with Id {e.SocketId} wants to join with token \"{playerTokenForLogging}\" again."
                );
                AfterPlayerConnectEvent?.Invoke(
                    this,
                    new AfterPlayerConnect(
                        _tokenToPlayerId[getPlayerInfoMessage.Token],
                        getPlayerInfoMessage.Token,
                        e.SocketId
                    )
                );
            }
            else
            {
                _logger.Information(
                    $"Adding player {_nextPlayerId} with token \"{playerTokenForLogging}\" to the game."
                );
                try
                {
                    if (
                        Game.AddPlayer(
                            new Player(
                                getPlayerInfoMessage.Token,
                                _nextPlayerId,
                                Constant.PLAYER_MAXIMUM_HEALTH,
                                Constant.PLAYER_SPEED_PER_TICK,
                                new Position(0, 0)
                            )
                        ) == false
                    )
                    {
                        _logger.Error(
                            $"Failed to add player with token \"{playerTokenForLogging}\" to the game."
                        );
                        return;
                    }

                    _tokenToPlayerId[getPlayerInfoMessage.Token] = _nextPlayerId;

                    AfterPlayerConnectEvent?.Invoke(this, new AfterPlayerConnect(
                        _nextPlayerId,
                        getPlayerInfoMessage.Token,
                        e.SocketId
                    ));

                    _nextPlayerId++;
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        $"Failed to add player with token \"{playerTokenForLogging}\" to the game: {ex.Message}"
                    );
                    _logger.Debug($"{ex}");
                }
            }
        }
        else
        {
            if (!_tokenToPlayerId.ContainsKey((e.Message as PerformMessage)!.Token))
            {
                _logger.Error($"Player with token \"{playerTokenForLogging}\" does not exist.");
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
                            $"Failed to perform action \"Abandon\" for player with token {playerTokenForLogging}: {ex.Message}"
                        );
                        _logger.Debug($"{ex}");
                    }
                    break;

                case PerformPickUpMessage performPickUpMessage:
                    try
                    {
                        Game.AllPlayers.Find(p => p.PlayerId == _tokenToPlayerId[performPickUpMessage.Token])?
                        .PlayerPickUp(
                            performPickUpMessage.TargetSupply,
                            performPickUpMessage.Num
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"Failed to perform action \"PickUp\" for player with token {playerTokenForLogging}: {ex.Message}"
                        );
                        _logger.Debug($"{ex}");
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
                            $"Failed to perform action \"PickUp\" for player with token {playerTokenForLogging}: {ex.Message}"
                        );
                        _logger.Debug($"{ex}");
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
                            $"Failed to perform action \"UseMedicine\" for player with token {playerTokenForLogging}: {ex.Message}"
                        );
                        _logger.Debug($"{ex}");
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
                            $"Failed to perform action \"UseGrenade\" for player with token {playerTokenForLogging}: {ex.Message}"
                        );
                        _logger.Debug($"{ex}");
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
                            $"Failed to perform action \"Move\" for player with token {playerTokenForLogging}: {ex.Message}"
                        );
                        _logger.Debug($"{ex}");
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
                            $"Failed to perform action \"Stop\" for player with token {playerTokenForLogging}: {ex.Message}"
                        );
                        _logger.Debug($"{ex}");
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
                            $"Failed to perform action \"Attack\" for player with token {playerTokenForLogging}: {ex.Message}"
                        );
                        _logger.Debug($"{ex}");
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
                            $"Failed to perform action \"ChooseOrigin\" for player with token {playerTokenForLogging}: {ex.Message}"
                        );
                        _logger.Debug($"{ex}");
                    }
                    break;

                default:
                    _logger.Error(
                        $"Unknown message type: {(e.Message.MessageType.Length > 32 ?
                        string.Concat(e.Message.MessageType.AsSpan(0, 32), "...")
                        : e.Message.MessageType)}"
                    );
                    break;
            }
        }
    }
}
