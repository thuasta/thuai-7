using GameServer.Geometry;

namespace GameServer.GameLogic;

public class WeaponProperties
{
    public int TicksUntilAvailable { get; init; }
}

public class WeaponFactory
{
    public static readonly string[] WeaponNames = {
        Constant.Names.FIST,
        Constant.Names.S686,
        Constant.Names.M16,
        Constant.Names.VECTOR,
        Constant.Names.AWM
    };

    public static IWeapon CreateFromItem(IItem item)
    {
        if (item.Kind != IItem.ItemKind.Weapon)
        {
            throw new ArgumentException($"Item kind {item.Kind} is not a weapon.");
        }

        if (item.AdditionalProperties is null)
        {
            return item.ItemSpecificName switch
            {
                Constant.Names.S686 => new ShotGun(),
                Constant.Names.M16 => new AssaultRifle(),
                Constant.Names.VECTOR => new SubMachineGun(),
                Constant.Names.AWM => new SniperRifle(),
                Constant.Names.FIST => throw new ArgumentException("Cannot create Fist from item."),
                _ => throw new ArgumentException($"Item specific name {item.ItemSpecificName} is not valid for weapon.")
            };
        }
        else if (item.AdditionalProperties is WeaponProperties properties)
        {
            return item.ItemSpecificName switch
            {
                Constant.Names.S686 => new ShotGun(properties.TicksUntilAvailable),
                Constant.Names.M16 => new AssaultRifle(properties.TicksUntilAvailable),
                Constant.Names.VECTOR => new SubMachineGun(properties.TicksUntilAvailable),
                Constant.Names.AWM => new SniperRifle(properties.TicksUntilAvailable),
                Constant.Names.FIST => throw new ArgumentException("Cannot create Fist from item."),
                _ => throw new ArgumentException($"Item specific name {item.ItemSpecificName} is not valid for weapon.")
            };
        }
        else
        {
            throw new ArgumentException(
                $"Additional properties {item.AdditionalProperties.GetType().Name} is not valid for weapon."
            );
        }
    }

    public static IItem ToItem(IWeapon weapon)
    {
        if (weapon is Fist || weapon.Name == Constant.Names.FIST)
        {
            throw new ArgumentException("Cannot create item from Fist.");
        }
        try
        {
            return new Item(IItem.ItemKind.Weapon, weapon.Name, 1)
            {
                AdditionalProperties = new WeaponProperties
                {
                    TicksUntilAvailable = weapon.TicksUntilAvailable
                }
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Cannot create item from weapon {weapon.Name}.", ex);
        }
    }
}

public class Fist : IWeapon
{
    public string Name { get; } = Constant.Names.FIST;
    public float Range { get; }
    public int Damage { get; }
    public int CoolDownTicks { get; }
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }
    public int RequiredBulletNum => 0;

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

    public Fist(int? ticksUntilAvailable = null)
    {
        Range = Constant.FIST_RANGE;
        Damage = Constant.FIST_DAMAGE;
        CoolDownTicks = Constant.FIST_COOLDOWNTICKS;
        TicksUntilAvailable = ticksUntilAvailable ?? 0;
    }
}

public class ShotGun : IWeapon
{
    public string Name { get; } = Constant.Names.S686;
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
    public int RequiredBulletNum => 3;
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
    public ShotGun(int? ticksUntilAvailable = null)
    {
        Range = Constant.S686_RANGE;
        BulletNum = Constant.S686_BULLET_NUM;
        DeltaDegree = Constant.S686_DELTA_DEG;
        Damage = Constant.S686_SINGLE_BULLET_DAMAGE;
        CoolDownTicks = Constant.S686_COOLDOWNTICKS;
        TicksUntilAvailable = ticksUntilAvailable ?? 0;
    }
}

public class SubMachineGun : IWeapon
{
    public string Name { get; } = Constant.Names.VECTOR;
    public float Range { get; }
    public int Damage { get; }
    public int CoolDownTicks { get; }
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }
    public int RequiredBulletNum => 1;
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
    public SubMachineGun(int? ticksUntilAvailable = null)
    {
        Range = Constant.VECTOR_RANGE;
        Damage = Constant.VECTOR_DAMAGE;
        CoolDownTicks = Constant.VECTOR_COOLDOWNTICKS;
        TicksUntilAvailable = ticksUntilAvailable ?? 0;
    }
}

public class SniperRifle : IWeapon
{
    public string Name { get; } = Constant.Names.AWM;
    public float Range { get; }
    public int Damage { get; }
    public int CoolDownTicks { get; }
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }
    public int RequiredBulletNum => 3;
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
    public SniperRifle(int? ticksUntilAvailable = null)
    {
        Range = Constant.AWM_RANGE;
        Damage = Constant.AWM_DAMAGE;
        CoolDownTicks = Constant.AWM_COOLDOWNTICKS;
        TicksUntilAvailable = ticksUntilAvailable ?? 0;
    }
}

public class AssaultRifle : IWeapon
{
    public string Name { get; } = Constant.Names.M16;
    public float Range { get; }
    public int Damage { get; }
    public int CoolDownTicks { get; }
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }
    public int RequiredBulletNum => 1;
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
    public AssaultRifle(int? ticksUntilAvailable = null)
    {
        Range = Constant.M16_RANGE;
        Damage = Constant.M16_DAMAGE;
        CoolDownTicks = Constant.M16_COOLDOWNTICKS;
        TicksUntilAvailable = ticksUntilAvailable ?? 0;
    }
}
