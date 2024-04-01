using GameServer.Engine.Shapes;

namespace GameServer.GameLogic;

public class WeaponFactory
{
    public static IWeapon CreateFromItem(IItem item)
    {
        if (item.Kind != IItem.ItemKind.Weapon)
        {
            throw new ArgumentException($"Item kind {item.Kind} is not a weapon.");
        }

        return item.ItemSpecificName switch
        {
            "FIST" => new Fist(),
            "S686" => new ShotGun(),
            "M16" => new AssaultRifle(),
            "VECTOR" => new SubMachineGun(),
            "AWM" => new SniperRifle(),
            _ => throw new ArgumentException($"Item specific id {item.ItemSpecificName} is not valid for weapon.")
        };
    }

    public static IItem ToItem(IWeapon weapon)
    {
        return weapon switch
        {
            Fist _ => new Item(IItem.ItemKind.Weapon, "FIST", 1),
            ShotGun _ => new Item(IItem.ItemKind.Weapon, "S686", 1),
            AssaultRifle _ => new Item(IItem.ItemKind.Weapon, "M16", 1),
            SubMachineGun _ => new Item(IItem.ItemKind.Weapon, "VECTOR", 1),
            SniperRifle _ => new Item(IItem.ItemKind.Weapon, "AWM", 1),
            _ => throw new ArgumentException($"Weapon is not of valid weapon-class.")
        };
    }
}

public class Fist : IWeapon
{
    // TODO: Implement
    public float Range { get; }
    public int Damage { get; }
    public int CoolDownTicks { get; }
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }

    public void Attack(IPlayer owner, Position target)
    {
        if (TicksUntilAvailable > 0) return;
        TicksUntilAvailable = CoolDownTicks;
        throw new NotImplementedException();
    }
    public void UpdateCoolDown()
    {
        if (TicksUntilAvailable > 0)
        {
            TicksUntilAvailable--;
        }
    }

    public Fist()
    {
        Range = Constant.FIST_RANGE;
        Damage = Constant.FIST_DAMAGE;
        CoolDownTicks = Constant.FIST_COOLDOWNTICKS;
        TicksUntilAvailable = 0;
    }
}

public class ShotGun : IWeapon
{
    private int BulletNum { get; }
    private int DeltaDegree { get; }
    public float Range { get; }
    public int Damage { get; }
    public int CoolDownTicks { get; }
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }
    public void Attack(IPlayer owner, Position target)
    {
        throw new NotImplementedException();
    }
    public void UpdateCoolDown()
    {
        if (TicksUntilAvailable > 0)
        {
            TicksUntilAvailable--;
        }
    }
    public ShotGun()
    {
        Range = Constant.S686_RANGE;
        BulletNum = Constant.S686_BULLET_NUM;
        DeltaDegree = Constant.S686_DELTA_DEG;
        Damage = Constant.S686_SINGLE_BULLET_DAMAGE;
        CoolDownTicks = Constant.S686_COOLDOWNTICKS;
        TicksUntilAvailable = 0;
    }
}

public class SubMachineGun : IWeapon
{
    // TODO: Implement
    public float Range { get; }
    public int Damage { get; }
    public int CoolDownTicks { get; }
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }
    public void Attack(IPlayer owner, Position target)
    {
        throw new NotImplementedException();
    }
    public void UpdateCoolDown()
    {
        if (TicksUntilAvailable > 0)
        {
            TicksUntilAvailable--;
        }
    }
    public SubMachineGun()
    {
        Range = Constant.VECTOR_RANGE;
        Damage = Constant.VECTOR_DAMAGE;
        CoolDownTicks = Constant.VECTOR_COOLDOWNTICKS;
        TicksUntilAvailable = 0;
    }
}

public class SniperRifle : IWeapon
{
    // TODO: Implement
    public float Range { get; }
    public int Damage { get; }
    public int CoolDownTicks { get; }
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }
    public void Attack(IPlayer owner, Position target)
    {
        throw new NotImplementedException();
    }
    public void UpdateCoolDown()
    {
        if (TicksUntilAvailable > 0)
        {
            TicksUntilAvailable--;
        }
    }
    public SniperRifle()
    {
        Range = Constant.AWM_RANGE;
        Damage = Constant.AWM_DAMAGE;
        CoolDownTicks = Constant.AWM_COOLDOWNTICKS;
        TicksUntilAvailable = 0;
    }
}

public class AssaultRifle : IWeapon
{
    // TODO: Implement
    public float Range { get; }
    public int Damage { get; }
    public int CoolDownTicks { get; }
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }
    public void Attack(IPlayer owner, Position target)
    {
        throw new NotImplementedException();
    }
    public void UpdateCoolDown()
    {
        if (TicksUntilAvailable > 0)
        {
            TicksUntilAvailable--;
        }
    }
    public AssaultRifle()
    {
        Range = Constant.M16_RANGE;
        Damage = Constant.M16_DAMAGE;
        CoolDownTicks = Constant.M16_COOLDOWNTICKS;
        TicksUntilAvailable = 0;
    }
}
