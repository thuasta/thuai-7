using static GameServer.GameLogic.IItem;

namespace GameServer.GameLogic;

public class Player : IPlayer
{
    // TODO: Implement
    public int Health { get; set; }
    public double Speed { get; set; }
    public Armor? PlayerArmor { get; set; }
    public Position PlayerPosition { get; set; }
    public IWeapon PlayerWeapon { get; set; }
    public IBackPack PlayerBackPack { get; set; }

    //生成构造函数
    public Player(int health, double speed, Position position)
    {
        Health = health;
        Speed = speed;
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

    public void playerMove(Position position)
    {
        PlayerPosition = position;
    }

    public bool playerAttack()
    {
        IItem? item = PlayerBackPack.FindItems(ItemKind.Bullet, "BULLET");
        if (item != null && item.Count > 0)
        {
            PlayerBackPack.RemoveItems(ItemKind.Bullet, "BULLET", 1);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool playerUseGrenade()
    {
        IItem? item = PlayerBackPack.FindItems(ItemKind.Grenade, "GRENADE");
        if (item != null && item.Count > 0)
        {
            PlayerBackPack.RemoveItems(ItemKind.Grenade, "GRENADE", 1);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool playerUseMedicine(string medicineName)
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
    public void TakeHeal(int heal)
    {
        throw new NotImplementedException();
    }

    public bool playerChangeWeapon()
    {
        // TODO:Implement
        throw new NotImplementedException();
    }
    public void SwitchWeapon(string weaponItemId)
    {
        //iterate player's backpack to find the weapon with weaponItemId
        //if found, set PlayerWeapon to the weapon
        //if not found, throw new ArgumentException("Weapon not found in backpack.");
        foreach (IItem item in PlayerBackPack.Items)
        {
            if (item.ItemSpecificName == weaponItemId)
            {
                PlayerWeapon = WeaponFactory.CreateFromItem(item);
                return;
            }
        }
    }
}
