public class Circle
{
    public Position Position { get; set; }
    public float Radius { get; set; }

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