using GameServer.Geometry;
using Serilog;
using static GameServer.GameLogic.IItem;

namespace GameServer.GameLogic;

public partial class Player
{
    public string Token { get; }
    public int PlayerId { get; set; }

    public double PlayerRadius { get; set; }

    public int MaxHealth { get; }
    public int Health { get; set; }
    public bool IsAlive => Health > 0;

    public DateTime? DieTime { get; set; } = null;
    public double Speed { get; set; }

    public Armor PlayerArmor { get; set; }

    public Position PlayerPosition { get; set; }
    public Position? PlayerTargetPosition { get; set; }

    public IWeapon PlayerWeapon { get; set; }
    public List<IWeapon> WeaponSlot { get; private set; } = new();

    public IBackPack PlayerBackPack { get; set; }
    public int MaxWeaponSlotSize => 1 + Constant.PLAYER_WEAPON_SLOT_SIZE;
    public bool IsWeaponSlotFull => WeaponSlot.Count >= MaxWeaponSlotSize;

    public int? LastPickUpOrAbandonTick { get; set; } = null;
    public int? LastUseGrenadeTick { get; set; } = null;
    public int? LastSwitchArmTick { get; set; } = null;

    private readonly ILogger _logger;

    //生成构造函数
    public Player(string token, int playerId, int maxHealth, double speed, Position position)
    {
        Token = token;
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
        _logger.Debug($"Attempting to teleport to ({position.x}, {position.y}).");

        PlayerTeleportEvent?.Invoke(this, new PlayerTeleportEventArgs(this, position));
    }

    public void TakeDamage(int damage, bool ignoreArmor = false)
    {
        if (IsAlive == false)
        {
            return;
        }

        if (damage < 0)
        {
            throw new ArgumentException("damage should be non-negative.");
        }

        if (PlayerArmor != null && ignoreArmor == false)
        {
            Health -= PlayerArmor.Hurt(damage);
            if (PlayerArmor.Health == 0)
            {
                PlayerArmor = Armor.DefaultArmor;
            }
        }
        else
        {
            Health -= damage;
        }

        Health = Math.Max(0, Health);

        if (Health == 0)
        {
            DieTime = DateTime.UtcNow;
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
        _logger.Debug($"Setting target position to ({destination.x}, {destination.y}).");

        PlayerTargetPosition = destination;
    }

    public void Stop()
    {
        _logger.Debug("Stopping.");

        PlayerTargetPosition = null;
    }

    public void PlayerAbandon(int number, (ItemKind itemKind, string itemSpecificName) abandonedSupplies)
    {
        _logger.Debug($"Attempting to abandon {number} {abandonedSupplies.itemSpecificName}(s).");

        if (IsAlive == false)
        {
            _logger.Error($"Failed to abandon items: Player {PlayerId} is already dead.");
            return;
        }

        PlayerAbandonEvent?.Invoke(this, new PlayerAbandonEventArgs(this, number, abandonedSupplies));
    }

    public void PlayerAttack(Position targetPosition)
    {
        _logger.Debug($"Attempting to attack ({targetPosition.x}, {targetPosition.y}).");

        if (IsAlive == false)
        {
            _logger.Error($"Failed to attack: Player {PlayerId} is already dead.");
            return;
        }

        PlayerAttackEvent?.Invoke(this, new PlayerAttackEventArgs(this, targetPosition));
    }

    public void PlayerUseGrenade(Position targetPosition)
    {
        _logger.Debug($"Attempting to use grenade at ({targetPosition.x}, {targetPosition.y}).");

        if (IsAlive == false)
        {
            _logger.Error($"Failed to use grenade: Player {PlayerId} is already dead.");
            return;
        }

        PlayerUseGrenadeEvent?.Invoke(this, new PlayerUseGrenadeEventArgs(this, targetPosition));

    }

    public void PlayerUseMedicine(string medicineName)
    {
        _logger.Debug($"Attempting to use medicine {medicineName}.");

        if (IsAlive == false)
        {
            _logger.Error($"Failed to use medicine: Player {PlayerId} is already dead.");
            return;
        }

        PlayerUseMedicineEvent?.Invoke(this, new PlayerUseMedicineEventArgs(this, medicineName));
    }

    public void PlayerSwitchArm(string weaponItemId)
    {
        _logger.Debug($"Attempting to switch weapon to {weaponItemId}.");

        if (IsAlive == false)
        {
            _logger.Error($"Failed to switch arm: Player {PlayerId} is already dead.");
            return;
        }

        PlayerSwitchArmEvent?.Invoke(this, new PlayerSwitchArmEventArgs(this, weaponItemId));
    }

    public void PlayerPickUp(string targetSupply, int numb)
    {
        _logger.Debug($"Attempting to pick up {numb} {targetSupply}(s).");

        if (IsAlive == false)
        {
            _logger.Error($"Failed to pick up: Player {PlayerId} is already dead.");
            return;
        }

        PlayerPickUpEvent?.Invoke(this, new PlayerPickUpEventArgs(this, targetSupply, numb));
    }
}
