namespace GameServer.Engine.Shapes;

public struct Circle<T>
{
    public Point<T> Center { get; set; }
    public T Radius { get; set; }

    public Circle(Point<T> center, T radius)
    {
        Center = center;
        Radius = radius;
    }
}
