using GameServer.Geometry;

namespace GameServer.GameLogic;

public partial class Game
{
    public List<Player> AllPlayers { get; private set; } = new();
    public int PlayerCount => AllPlayers.Count;

    public List<Recorder.IRecord> _events = new();

    public void AddPlayer(Player player)
    {
        if (Stage != GameStage.Waiting)
        {
            _logger.Error("Cannot add player: The game is already started.");
            return;
        }

        AllPlayers.Add(player);
        SubscribePlayerEvents(player);
    }

    public void RemovePlayer(Player player)
    {
        AllPlayers.Remove(player);
    }

    public List<Player> GetPlayers()
    {
        return AllPlayers;
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

            // Update motion of players
            if (player.PlayerTargetPosition != null)
            {
                if ((player.PlayerTargetPosition - player.PlayerPosition).Length() <= 1e-6)
                {
                    player.PlayerTargetPosition = null;
                }
                else
                {
                    // Calculate the direction of the player (normalized vector
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
                        Position realEndPosition = GameMap.GetRealEndPositon(player.PlayerPosition, expectedEndPosition);
                        player.PlayerPosition = realEndPosition;
                    }
                }
            }
        }
    }
}
