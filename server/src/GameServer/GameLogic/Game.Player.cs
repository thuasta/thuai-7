namespace GameServer.GameLogic;

public partial class Game
{
    private readonly List<Player> _allPlayers = new();

    public void AddPlayer(Player player)
    {
        _allPlayers.Add(player);
    }

    public void RemovePlayer(Player player)
    {
        _allPlayers.Remove(player);
    }

    public List<Player> GetPlayers()
    {
        return _allPlayers;
    }

    private void SubscribePlayerEvents()
    {
        foreach (Player player in _allPlayers)
        {
            player.PlayerAbandonEvent += OnPlayerAbandon;
            player.PlayerAttackEvent += OnPlayerAttack;
            player.PlayerPickUpEvent += OnPlayerPickUp;
            player.PlayerSwitchArmEvent += OnPlayerSwitchArm;
            player.PlayerUseGrenadeEvent += OnPlayerUseGrenade;
        }
    }
    private void OnPlayerAbandon(object? sender, Player.PlayerAbandonEventArgs e)
    {
        foreach ((IItem.ItemKind itemKind, string itemSpecificName) in e.AbandonedSupplies)
        {
            IItem? item = e.Player.PlayerBackPack.FindItems(itemKind, itemSpecificName);
            if (item != null && item.Count >= e.Number)
            {
                // Remove abandon items from the backpack
                e.Player.PlayerBackPack.RemoveItems(itemKind, itemSpecificName, e.Number);

                // Add abandon items to the ground
                // Get the block at the position of the player
                Position playerPosition = e.Player.PlayerPosition;
                int playerIntX = (int)playerPosition.x;
                int playerIntY = (int)playerPosition.y;
                _map.AddSupplies(playerIntX, playerIntY, new Item(itemKind, itemSpecificName, e.Number));
            }
        }
    }
    private void OnPlayerAttack(object? sender, Player.PlayerAttackEventArgs e)
    {
        // Check if the player has enough bullets
        IItem? item = e.Player.PlayerBackPack.FindItems(IItem.ItemKind.Bullet, "BULLET");
    }
    private void OnPlayerPickUp(object? sender, Player.PlayerPickUpEventArgs e)
    {

    }
    private void OnPlayerSwitchArm(object? sender, Player.PlayerSwitchArmEventArgs e)
    {

    }
    private void OnPlayerUseGrenade(object? sender, Player.PlayerUseGrenadeEventArgs e)
    {

    }
    private void UpdatePlayers()
    {
        foreach (Player player in _allPlayers)
        {
            // 
        }
    }
}
