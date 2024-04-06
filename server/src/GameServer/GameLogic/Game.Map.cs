namespace GameServer.GameLogic;

public partial class Game
{
    private readonly Map _map;

    private void UpdateMap()
    {
        // Update Safezone
        _map.SafeZone.Update();

        // Check if players are in safezone
        foreach (Player player in _allPlayers)
        {
            if (!_map.SafeZone.IsInSafeZone(player.PlayerPosition))
            {
                player.Health -= _map.SafeZone.DamageOutside;
            }
        }
    }
}
