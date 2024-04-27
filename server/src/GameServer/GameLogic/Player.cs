using GameServer.Geometry;
using Serilog;
using static GameServer.GameLogic.IItem;

namespace GameServer.GameLogic;

public partial class Player
{
    public int PlayerId { get; set; }

    public double PlayerRadius { get; set; }

    public int MaxHealth { get; }
    public int Health { get; set; }
    public bool IsAlive => Health > 0;

    public double Speed { get; set; }

    public Armor PlayerArmor { get; set; }

    public Position PlayerPosition { get; set; }
    public Position? PlayerTargetPosition { get; set; }

    public IWeapon PlayerWeapon { get; set; }
    public List<IWeapon> WeaponSlot { get; private set; } = new();

    public IBackPack PlayerBackPack { get; set; }
    public int MaxWeaponSlotSize => 1 + Constant.PLAYER_WEAPON_SLOT_SIZE;
    public bool IsWeaponSlotFull => WeaponSlot.Count >= MaxWeaponSlotSize;

    private readonly ILogger _logger;

    //生成构造函数
    public Player(int playerId, int maxHealth, double speed, Position position)
    {
        PlayerId = playerId;
        Health = maxHealth;
        MaxHealth = maxHealth;
        Speed = speed;
        PlayerRadius = Constant.PLAYER_COLLISION_BOX;
        PlayerPosition = position;
        PlayerArmor = new Armor(Constant.Names.NO_ARMOR, Constant.NO_ARMOR_DEFENSE);
        PlayerBackPack = new BackPack(Constant.PLAYER_INITIAL_BACKPACK_SIZE);

        IWeapon defaultWeapon = IWeapon.DefaultWeapon;
        WeaponSlot.Add(defaultWeapon);
        PlayerWeapon = WeaponSlot[0];

        _logger = Log.ForContext("Component", $"Player {playerId}");
    }

    public void Teleport(Position position)
    {
        PlayerTeleportEvent?.Invoke(this, new PlayerTeleportEventArgs(this, position));
    }

    public void TakeDamage(int damage, bool ignoreArmor = false)
    {
        if (IsAlive == false)
        {
            _logger.Error($"Failed to take damage: Player {PlayerId} is already dead.");
            return;
        }
        if (damage < 0)
        {
            _logger.Error($"Damage should be non-negative, but actually {damage}.");
            return;
        }

        if (PlayerArmor != null && ignoreArmor == false)
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
            _logger.Error($"Failed to take heal: Player {PlayerId} is already dead.");
            return;
        }
        if (heal < 0)
        {
            _logger.Error($"Heal should be non-negative, but actually {heal}.");
            return;
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

    public void PlayerAbandon(int number, (ItemKind itemKind, string itemSpecificName) abandonedSupplies)
    {
        if (IsAlive == false)
        {
            _logger.Error($"Failed to abandon items: Player {PlayerId} is already dead.");
            return;
        }

        PlayerAbandonEvent?.Invoke(this, new PlayerAbandonEventArgs(this, number, abandonedSupplies));
    }

    public void PlayerAttack(Position targetPosition)
    {
        if (IsAlive == false)
        {
            _logger.Error($"Failed to attack: Player {PlayerId} is already dead.");
            return;
        }

        PlayerAttackEvent?.Invoke(this, new PlayerAttackEventArgs(this, targetPosition));
    }

    public void PlayerUseGrenade(Position targetPosition)
    {
        if (IsAlive == false)
        {
            _logger.Error($"Failed to use grenade: Player {PlayerId} is already dead.");
            return;
        }

        PlayerUseGrenadeEvent?.Invoke(this, new PlayerUseGrenadeEventArgs(this, targetPosition));

    }

    public void PlayerUseMedicine(string medicineName)
    {
        if (IsAlive == false)
        {
            _logger.Error($"Failed to use medicine: Player {PlayerId} is already dead.");
            return;
        }

        PlayerUseMedicineEvent?.Invoke(this, new PlayerUseMedicineEventArgs(this, medicineName));
    }

    public void PlayerSwitchArm(string weaponItemId)
    {
        if (IsAlive == false)
        {
            _logger.Error($"Failed to switch arm: Player {PlayerId} is already dead.");
            return;
        }

        PlayerSwitchArmEvent?.Invoke(this, new PlayerSwitchArmEventArgs(this, weaponItemId));
    }

    public void PlayerPickUp(string targetSupply, int numb)
    {
        if (IsAlive == false)
        {
            _logger.Error($"Failed to pick up: Player {PlayerId} is already dead.");
            return;
        }

        PlayerPickUpEvent?.Invoke(this, new PlayerPickUpEventArgs(this, targetSupply, numb));
    }
}
