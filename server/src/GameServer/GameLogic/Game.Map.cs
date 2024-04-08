namespace GameServer.GameLogic;

public partial class Game
{
    public Map GameMap;

    private void UpdateMap()
    {
        // Update Safezone
        GameMap.SafeZone.Update();
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
        foreach (Player player in AllPlayers)
        {
            if (!GameMap.SafeZone.IsInSafeZone(player.PlayerPosition))
            {
                player.Health -= GameMap.SafeZone.DamageOutside;
            }
        }
    }
}
