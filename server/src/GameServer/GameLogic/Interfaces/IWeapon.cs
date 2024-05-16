using GameServer.Geometry;

namespace GameServer.GameLogic;

/// <summary>
/// Interface for a weapon.
/// </summary>
public interface IWeapon
{
    public static IWeapon DefaultWeapon => new Fist();

    /// <summary>
    /// The name of the weapon.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The range of the weapon.
    /// </summary>
    public float Range { get; }

    /// <summary>
    /// The damage of the weapon.
    /// </summary>
    public int Damage { get; }

    /// <summary>
    /// The number of ticks between two attacks.
    /// </summary>
    public int CoolDownTicks { get; }

    /// <summary>
    /// Whether the weapon is available for attack at current tick.
    /// </summary>
    public bool IsAvailable { get; }

    /// <summary>
    /// The number of ticks until the weapon is available for attack.
    /// </summary>
    public int TicksUntilAvailable { get; }

    /// <summary>
    /// Whether the weapon requires bullets.
    /// </summary>
    public int RequiredBulletNum { get; }

    /// <summary>
    /// Get Bullet Directions.
    /// </summary>
    /// <param name="target"></param>
    /// <returns>The normalized directions of the bullets</returns>
    public List<Position>? GetBulletDirections(Position start, Position target);

    /// <summary>
    /// Update the cooldown of the weapon.
    /// Should be called every tick.
    /// </summary>
    public void UpdateCoolDown();
}
