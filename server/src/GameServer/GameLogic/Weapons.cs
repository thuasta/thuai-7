using GameServer.Engine.Shapes;

namespace GameServer.GameLogic;

public class WeaponFactory
{
    public static IWeapon CreateFromItem(IItem Item)
    {
        if (Item.Kind != IItem.ItemKind.Weapon)
        {
            throw new ArgumentException($"Item kind {Item.Kind} is not a weapon.");
        }

        return Item.ItemSpecificId switch
        {
            // TODO: Implement
            _ => throw new NotImplementedException()
        };
    }

    public static IItem ToItem(IWeapon weapon)
    {
        return weapon switch
        {
            // TODO: Create items from weapons
            _ => throw new NotImplementedException()
        };
    }
}

public class Fist : IWeapon
{
    // TODO: Implement
    public float Range => throw new NotImplementedException();
    public int Damage => throw new NotImplementedException();
    public int CoolDownTicks => throw new NotImplementedException();
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }

    public void Attack(IPlayer owner, Point<float> target)
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
}

public class ShotGun : IWeapon
{
    // TODO: Implement
    public float Range => throw new NotImplementedException();
    public int Damage => throw new NotImplementedException();
    public int CoolDownTicks => throw new NotImplementedException();
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }
    public void Attack(IPlayer owner, Point<float> target)
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
}

public class SubMachineGun : IWeapon
{
    // TODO: Implement
    public float Range => throw new NotImplementedException();
    public int Damage => throw new NotImplementedException();
    public int CoolDownTicks => throw new NotImplementedException();
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }
    public void Attack(IPlayer owner, Point<float> target)
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
}

public class SniperRifle : IWeapon
{
    // TODO: Implement
    public float Range => throw new NotImplementedException();
    public int Damage => throw new NotImplementedException();
    public int CoolDownTicks => throw new NotImplementedException();
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }
    public void Attack(IPlayer owner, Point<float> target)
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
}

public class AssaultRifle : IWeapon
{
    // TODO: Implement
    public float Range => throw new NotImplementedException();
    public int Damage => throw new NotImplementedException();
    public int CoolDownTicks => throw new NotImplementedException();
    public bool IsAvailable
    {
        get => (TicksUntilAvailable == 0);
    }
    public int TicksUntilAvailable { get; private set; }
    public void Attack(IPlayer owner, Point<float> target)
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
}
