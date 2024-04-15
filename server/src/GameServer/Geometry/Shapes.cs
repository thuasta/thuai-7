using GameServer.GameLogic;

namespace GameServer.Geometry.Shapes;

public readonly struct Circle
{
    public Position Center { get; }
    public double Radius { get; }
    public Circle(Position center, double radius)
    {
        Center = new(center.x, center.y);
        Radius = radius;
    }
}

public readonly struct Segment
{
    public Position Start { get; }
    public Position End { get; }
    public Segment(Position start, Position end)
    {
        Start = new(start.x, start.y);
        End = new(end.x, end.y);
    }
}
