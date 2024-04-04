namespace GameServer.GameLogic;

public partial class Game
{
    private readonly List<Grenade> _allGrenades = new();

    public void AddGrenade(Grenade grenade)
    {
        _allGrenades.Add(grenade);
    }

    public void UpdateGrenades()
    {
        // Update all grenades
        foreach (Grenade grenade in _allGrenades)
        {
            grenade.Explode(CurrentTick, _allPlayers.ToArray(), _map);
        }
    }
}
