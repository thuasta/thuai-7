namespace GameServer.GameLogic;

public partial class Game
{
    private readonly List<Grenade> _allGrenades = new();

    private void UpdateGrenades()
    {
        // Remove all exploded grenades
        for (int i = 0; i < _allGrenades.Count; i++)
        {
            if (_allGrenades[i].HasExploded == true)
            {
                _allGrenades.RemoveAt(i);
                i--;
            }
        }

        // Update all grenades
        foreach (Grenade grenade in _allGrenades)
        {
            grenade.Explode(CurrentTick, AllPlayers.ToArray(), GameMap, _events);
        }
    }
}
