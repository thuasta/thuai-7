namespace GameServer.GameLogic;

public partial class Game
{
    private readonly List<Grenade> _allGrenades = new();

    private void UpdateGrenades()
    {
        // Update all grenades
        foreach (Grenade grenade in _allGrenades)
        {
            grenade.Explode(CurrentTick, AllPlayers.ToArray(), GameMap, _events);
        }
    }
}
