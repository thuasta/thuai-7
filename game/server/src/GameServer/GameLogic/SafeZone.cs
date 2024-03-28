using GameServer.Engine.Shapes;

namespace GameServer.GameLogic;

public class SafeZone : ISafeZone
{
    public Position Center { get; private set; }
    public float MaxRadius { get; }
    public float Radius { get; private set; }
    public float RadiusChangedPerTick
    {
        get => (MaxRadius / (float)TicksUntilDisappear);
    }
    public int TicksUntilDisappear { get; }

    private readonly Random _random = new();

    /// <summary>
    /// Constructor of the safe zone.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="maxRadius"></param>
    /// <param name="ticksUntilDisappear"></param>
    public SafeZone(Position center, float maxRadius, int ticksUntilDisappear)
    {
        Center = center;
        MaxRadius = maxRadius;
        Radius = MaxRadius;
        TicksUntilDisappear = ticksUntilDisappear;
    }

    public void Update()
    {
        // Update center. Randomly move the cehnter of the safe zone.
        double newX = (float)(_random.NextDouble() - 0.5) * RadiusChangedPerTick + Center.x;
        double newY = (float)(_random.NextDouble() - 0.5) * RadiusChangedPerTick + Center.y;
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
