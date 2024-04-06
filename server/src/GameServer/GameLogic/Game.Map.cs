namespace GameServer.GameLogic;

public partial class Game
{
    private readonly Map _map;

    private void UpdateMap()
    {
        // Update Safezone
        _map.SafeZone.Update();
        Recorder.SafeZone record = new() {
            Data = new() {
                center = new() {
                    x = _map.SafeZone.Center.x,
                    y = _map.SafeZone.Center.y
                },
            radius = _map.SafeZone.Radius
            }
        };

        _recorder?.Record(record);

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
