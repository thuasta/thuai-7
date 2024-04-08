using Serilog;
using static GameServer.GameLogic.IItem;

namespace GameServer.GameLogic;

public partial class Player : IPlayer
{
    public int Id { get; set; }
    public int MaxHealth { get; }
    public int Health { get; set; }
    public double Speed { get; set; }
    public double PlayerRadius { get; set; }
    public Armor PlayerArmor { get; set; }
    public Position PlayerPosition { get; set; }
    public Position? PlayerTargetPosition { get; set; }
    public IWeapon PlayerWeapon { get; set; }
    public IBackPack PlayerBackPack { get; set; }
    public int PlayerId { get; set; }
    public bool IsAlive => Health > 0;

    private readonly ILogger _logger;

    //生成构造函数
    public Player(int id, int maxHealth, double speed, Position position)
    {
        Id = id;
        Health = maxHealth;
        MaxHealth = maxHealth;
        Speed = speed;
        PlayerRadius = Constant.PLAYER_COLLISION_BOX;
        PlayerPosition = position;
        PlayerArmor = new Armor("NO_ARMOR", Constant.NO_ARMOR_DEFENSE);
        PlayerWeapon = new Fist();
        PlayerBackPack = new BackPack(Constant.PLAYER_INITIAL_BACKPACK_SIZE);

        _logger = Log.ForContext("Component", $"Player {id}");
    }

    public void TakeDamage(int damage)
    {
        if (IsAlive == false)
        {
            _logger.Error($"Failed to take damage: Player {Id} is already dead.");
            return;
        }

        if (damage < 0)
        {
            _logger.Error($"Damage should be non-negative, but actually {damage}.");
        }
        if (PlayerArmor != null)
        {
            Health -= PlayerArmor.Hurt(damage);
        }
        else
        {
            Health -= damage;
        }
    }

    public void TakeHeal(int heal)
    {
        if (IsAlive == false)
        {
            _logger.Error($"Failed to take heal: Player {Id} is already dead.");
            return;
        }

        if (heal < 0)
        {
            throw new ArgumentException($"Heal should be non-negative, but actually {heal}.");
        }
        if (Health + heal > MaxHealth)
        {
            Health = MaxHealth;
        }
        else
        {
            Health += heal;
        }
    }

    public void MoveTo(Position destination)
    {
        PlayerTargetPosition = destination;
    }

    public void Stop()
    {
        PlayerTargetPosition = null;
    }

    public bool TryPickUpItem(Item item)
    {
        if (IsAlive == false)
        {
            _logger.Error($"Failed to pick up item {item.ItemSpecificName}: Player {Id} is already dead.");
            return false;
        }

        try
        {
            PlayerBackPack.AddItems(item.Kind, item.ItemSpecificName, item.Count);
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        return true;
    }

    public void PlayerAbandon(int number, List<(ItemKind itemKind, string itemSpecificName)> abandonedSupplies)
    {
        if (IsAlive == false)
        {
            _logger.Error($"Failed to abandon items: Player {Id} is already dead.");
            return;
        }

        PlayerAbandonEvent?.Invoke(this, new PlayerAbandonEventArgs(this, number, abandonedSupplies));
    }

    public void PlayerAttack(Position targetPosition)
    {
        if (IsAlive == false)
        {
            _logger.Error($"Failed to abandon items: Player {Id} is already dead.");
            return;
        }

        PlayerAttackEvent?.Invoke(this, new PlayerAttackEventArgs(this, targetPosition));
    }

    public void PlayerUseGrenade(Position targetPosition)
    {
        if (IsAlive == false)
        {
            _logger.Error($"Failed to abandon items: Player {Id} is already dead.");
            return;
        }

        PlayerUseGrenadeEvent?.Invoke(this, new PlayerUseGrenadeEventArgs(this, targetPosition));

    }

    public void PlayerUseMedicine(string medicineName)
    {
        if (IsAlive == false)
        {
            _logger.Error($"Failed to abandon items: Player {Id} is already dead.");
            return;
        }

        PlayerUseMedicineEvent?.Invoke(this, new PlayerUseMedicineEventArgs(this, medicineName));
    }


    public void PlayerSwitchArm(string weaponItemId)
    {
        if (IsAlive == false)
        {
            _logger.Error($"Failed to abandon items: Player {Id} is already dead.");
            return;
        }

        PlayerSwitchArmEvent?.Invoke(this, new PlayerSwitchArmEventArgs(this, weaponItemId));
    }

    public void PlayerPickUp(string targetSupply, Position targetPosition, int numb)
    {
        if (IsAlive == false)
        {
            _logger.Error($"Failed to abandon items: Player {Id} is already dead.");
            return;
        }

        PlayerPickUpEvent?.Invoke(this, new PlayerPickUpEventArgs(this, targetSupply, targetPosition, numb));
    }
}
