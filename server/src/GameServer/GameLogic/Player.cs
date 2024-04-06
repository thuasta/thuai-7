using static GameServer.GameLogic.IItem;

namespace GameServer.GameLogic;

public partial class Player : IPlayer
{
    // TODO: Implement
    public int Health { get; set; }
    public double Speed { get; set; }
    public double PlayerRadius { get; set; }
    public Armor? PlayerArmor { get; set; }
    public Position PlayerPosition { get; set; }
    public Position? PlayerTargetPosition { get; set; }
    public IWeapon PlayerWeapon { get; set; }
    public IBackPack PlayerBackPack { get; set; }
    public int PlayerId { get; set; }

    //生成构造函数
    public Player(int health, double speed, Position position)
    {
        Health = health;
        Speed = speed;
        PlayerRadius = Constant.PLAYER_COLLISION_BOX;
        PlayerPosition = position;
        PlayerArmor = null;
        PlayerWeapon = new Fist();
        PlayerBackPack = new BackPack(Constant.PLAYER_INITIAL_BACKPACK_SIZE);
    }


    public void TakeDamage(int damage)
    {
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
        Health += heal;
    }
    public void Move(Position position)
    {
        PlayerPosition = position;
    }

    public bool TryPickUpItem(Item item)
    {
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
        PlayerAbandonEvent?.Invoke(this, new PlayerAbandonEventArgs(this, number, abandonedSupplies));
    }

    public void PlayerAttack(Position targetPosition)
    {
        PlayerAttackEvent?.Invoke(this, new PlayerAttackEventArgs(this, targetPosition));
    }

    public void PlayerUseGrenade(Position targetPosition)
    {
        PlayerUseGrenadeEvent?.Invoke(this, new PlayerUseGrenadeEventArgs(this, targetPosition));

    }

    public bool PlayerUseMedicine(string medicineName)
    {
        IItem? item = PlayerBackPack.FindItems(ItemKind.Medicine, medicineName);
        if (item != null && item.Count > 0)
        {
            PlayerBackPack.RemoveItems(ItemKind.Medicine, medicineName, 1);

            Health += MedicineFactory.CreateFromItem(item).Heal;

            return true;
        }
        else
        {
            return false;
        }
    }


    public void PlayerSwitchArm(string weaponItemId)
    {
        PlayerSwitchArmEvent?.Invoke(this, new PlayerSwitchArmEventArgs(this, weaponItemId));
    }

    public void PlayerPickUp(string targetSupply, Position targetPosition, int numb)
    {
        PlayerPickUpEvent?.Invoke(this, new PlayerPickUpEventArgs(this, targetSupply, targetPosition,numb));
    }
}
