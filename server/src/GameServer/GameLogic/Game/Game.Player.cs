using GameServer.Geometry;

namespace GameServer.GameLogic;

public partial class Game
{
    public List<Player> AllPlayers { get; private set; } = new();
    public int PlayerCount => AllPlayers.Count;

    public List<Recorder.IRecord> _events = new();

    public bool AddPlayer(Player player)
    {
        if (Stage != GameStage.Waiting)
        {
            _logger.Error("Cannot add player: The game is already started.");
            return false;
        }

        try
        {
            lock (_lock)
            {
                AllPlayers.Add(player);
                SubscribePlayerEvents(player);
                return true;
            }
        }
        catch (Exception e)
        {
            _logger.Error($"Cannot add player: {e.Message}");
            _logger.Debug($"{e}");
            return false;
        }
    }

    public void RemovePlayer(Player player)
    {
        try
        {
            lock (_lock)
            {
                AllPlayers.Remove(player);
            }
        }
        catch (Exception e)
        {
            _logger.Error($"Cannot remove player: {e.Message}");
            _logger.Debug($"{e}");
        }
    }

    private void UpdatePlayers()
    {
        foreach (Player player in AllPlayers)
        {
            // Update cooldown of weapons
            foreach (IWeapon weapon in player.WeaponSlot)
            {
                weapon.UpdateCoolDown();
            }

            if (GameMap.GetBlock(player.PlayerPosition) is null
                || GameMap.GetBlock(player.PlayerPosition)?.IsWall == true)
            {
                _logger.Warning($"Player {player.PlayerId} is stuck in wall. Bouncing player back.");

                Position[] possiblePositions = [
                    player.PlayerPosition + new Position(0, Constant.BOUNCE_DISTANCE),
                    player.PlayerPosition + new Position(0, -Constant.BOUNCE_DISTANCE),
                    player.PlayerPosition + new Position(Constant.BOUNCE_DISTANCE, 0),
                    player.PlayerPosition + new Position(-Constant.BOUNCE_DISTANCE, 0),
                    player.PlayerPosition + new Position(Constant.BOUNCE_DISTANCE, Constant.BOUNCE_DISTANCE),
                    player.PlayerPosition + new Position(Constant.BOUNCE_DISTANCE, -Constant.BOUNCE_DISTANCE),
                    player.PlayerPosition + new Position(-Constant.BOUNCE_DISTANCE, Constant.BOUNCE_DISTANCE),
                    player.PlayerPosition + new Position(-Constant.BOUNCE_DISTANCE, -Constant.BOUNCE_DISTANCE)
                ];

                foreach (Position position in possiblePositions)
                {
                    if (GameMap.GetBlock(position) is not null
                        && GameMap.GetBlock(position)?.IsWall == false)
                    {
                        player.PlayerPosition = position;
                        break;
                    }
                }
            }

            // Update motion of players
            if (player.PlayerTargetPosition != null)
            {
                // Calculate the direction of the player (normalized vector)
                Position direction = (player.PlayerTargetPosition - player.PlayerPosition).Normalize();
                if (direction.Length() == 0)
                {
                    player.PlayerTargetPosition = null;
                }
                else
                {
                    Position expectedEndPosition = player.PlayerPosition + direction * player.Speed;
                    if ((direction * player.Speed).Length()
                        >= (player.PlayerTargetPosition - player.PlayerPosition).Length())
                    {
                        expectedEndPosition = player.PlayerTargetPosition;
                    }

                    Position realEndPosition = GameMap.GetRealEndPositon(
                        player.PlayerPosition, expectedEndPosition, independentAxisCauculating: true
                    );
                    player.PlayerPosition = realEndPosition;
                    if (Position.Distance(player.PlayerPosition, player.PlayerTargetPosition) < Constant.DISTANCE_ERROR)
                    {
                        player.PlayerTargetPosition = null;
                    }
                }
            }
        }
    }
}
