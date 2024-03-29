using GameServer.Engine.Shapes;

namespace GameServer.GameLogic;

/// <summary>
/// Interface for a weapon.
/// </summary>
public interface IWeapon
{
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
    /// Attack the target.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="target"></param>
    public void Attack(IPlayer owner, Point<float> target);

    /// <summary>
    /// Update the cooldown of the weapon.
    /// Should be called every tick.
    /// </summary>
    public void UpdateCoolDown();
}
