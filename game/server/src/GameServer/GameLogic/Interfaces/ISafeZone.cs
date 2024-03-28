using GameServer.Engine.Shapes;

namespace GameServer.GameLogic;

public interface ISafeZone
{
    /// <summary>
    /// The center of the safe zone.
    /// </summary>
    public Position Center { get; }

    /// <summary>
    /// The maximum radius of the safe zone.
    /// </summary>
    public float MaxRadius { get; }

    /// <summary>
    /// The radius of the safe zone.
    /// </summary>
    public float Radius { get; }

    /// <summary>
    /// The rate at which the radius of the safe zone changes per tick.
    /// </summary>
    public float RadiusChangedPerTick { get; }

    /// <summary>
    /// The number of ticks until the safe zone disappears.
    /// </summary>
    public int TicksUntilDisappear { get; }

    /// <summary>
    /// Update the safe zone.
    /// Should be called every tick.
    /// </summary>
    public void Update();

    /// <summary>
    /// Whether the point is in the safe zone or not.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool IsInSafeZone(Position point);
}
