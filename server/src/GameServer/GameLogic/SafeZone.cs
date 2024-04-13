using GameServer.Geometry;

namespace GameServer.GameLogic;

public class SafeZone : ISafeZone
{
    public Position Center { get; private set; }
    public int DamageOutside { get; }
    public float MaxRadius { get; }
    public float Radius { get; private set; }
    public float RadiusChangedPerTick
    {
        get => (MaxRadius / (float)TicksUntilDisappear);
    }
    public int TicksUntilDisappear { get; }

    private readonly Random _random = new();

    private readonly Position _direction;

    /// <summary>
    /// Constructor of the safe zone.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="maxRadius"></param>
    /// <param name="ticksUntilDisappear"></param>
    public SafeZone(Position center, float maxRadius, int ticksUntilDisappear, int damageOutside)
    {
        Center = center;
        MaxRadius = maxRadius;
        Radius = MaxRadius;
        TicksUntilDisappear = ticksUntilDisappear;
        DamageOutside = damageOutside;

        _direction = new Position(_random.NextDouble() - 0.5, _random.NextDouble() - 0.5).Normalize();
    }

    public void Update()
    {
        // Update center. Randomly move the center of the safe zone.
        double newX = (float)(_direction.x * _random.NextDouble()) * RadiusChangedPerTick + Center.x;
        double newY = (float)(_direction.y * _random.NextDouble()) * RadiusChangedPerTick + Center.y;
        Center = new(newX, newY);

        // Update radius
        if (Math.Abs(Radius) < 0.0001)
        {
            return;
        }
        Radius -= RadiusChangedPerTick;
        if (Radius < 0F)
        {
            Radius = 0F;
        }
    }

    public bool IsInSafeZone(Position point)
    {
        float distance = (float)Math.Sqrt(Math.Pow(Center.x - point.x, 2) + Math.Pow(Center.y - point.y, 2));
        return distance <= Radius;
    }
}
