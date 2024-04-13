public class Circle
{
    public Position Position { get; }
    public float Radius { get; }

    public Circle(Position position, float radius)
    {
        Position = position;
        Radius = radius;
    }

    public void Update(Position position, float radius)
    {
        Position = position;
        Radius = radius;
    }
}