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
    public string Name { get; } = "FIST";
    public float Range { get; }
    public int Damage { get; }
    public int CoolDownTicks { get; }
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }

    public List<Position>? GetBulletDirections(Position start, Position target)
    {
        if (TicksUntilAvailable > 0) return null;

        TicksUntilAvailable = CoolDownTicks;
        return new List<Position> { (target - start).Normalize() };
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
    public string Name { get; } = "S686";
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
    public List<Position>? GetBulletDirections(Position start, Position target)
    {
        if (TicksUntilAvailable > 0) return null;

        TicksUntilAvailable = CoolDownTicks;

        // Calculate the direction of the bullets
        List<Position> directions = new List<Position>();

        for (int i = 0; i < BulletNum; i++)
        {
            // Calculate the direction based on the root direction: (target - start)
            Position rootDirection = target - start;
            double angle = Math.Atan2(rootDirection.y, rootDirection.x);
            double deltaAngle = (i - BulletNum / 2) * DeltaDegree * Math.PI / 180;
            Position direction = new Position(Math.Cos(angle + deltaAngle), Math.Sin(angle + deltaAngle));
            directions.Add(direction);
        }

        return directions;
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
    public string Name { get; } = "VECTOR";
    public float Range { get; }
    public int Damage { get; }
    public int CoolDownTicks { get; }
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }
    public List<Position>? GetBulletDirections(Position start, Position target)
    {
        if (TicksUntilAvailable > 0) return null;

        TicksUntilAvailable = CoolDownTicks;
        return new List<Position> { (target - start).Normalize() };
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
    public string Name { get; } = "AWM";
    public float Range { get; }
    public int Damage { get; }
    public int CoolDownTicks { get; }
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }
    public List<Position>? GetBulletDirections(Position start, Position target)
    {
        if (TicksUntilAvailable > 0) return null;

        TicksUntilAvailable = CoolDownTicks;
        return new List<Position> { (target - start).Normalize() };
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
    public string Name { get; } = "M16";
    public float Range { get; }
    public int Damage { get; }
    public int CoolDownTicks { get; }
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }
    public List<Position>? GetBulletDirections(Position start, Position target)
    {
        if (TicksUntilAvailable > 0) return null;

        TicksUntilAvailable = CoolDownTicks;
        return new List<Position> { (target - start).Normalize() };
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
