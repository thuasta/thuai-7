namespace GameServer.GameLogic;

public partial class Game
{
    public Map GameMap;

    private void UpdateMap()
    {
        // Update Safezone
        GameMap.SafeZone.Update();

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
