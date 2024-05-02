namespace GameServer.GameLogic;

public partial class Game
{
    public Map GameMap;

    private void UpdateMap()
    {
        // Update Safezone
        GameMap.SafeZone.Update();
        Recorder.SafeZone record = new()
        {
            Data = new()
            {
                center = new()
                {
                    x = GameMap.SafeZone.Center.x,
                    y = GameMap.SafeZone.Center.y
                },
                radius = GameMap.SafeZone.Radius
            }
        };

        _recorder?.Record(record);

        // Check if players are in safezone
        foreach (Player player in AllPlayers)
        {
            if (GameMap.SafeZone.IsInSafeZone(player.PlayerPosition) == false)
            {
                player.TakeDamage(GameMap.SafeZone.DamageOutside, ignoreArmor: true);
            }
        }
    }
}
