namespace GameServer.Engine.Shapes;

public struct Segment<T>
{
    public Point<T> Point1 { get; set; }
    public Point<T> Point2 { get; set; }

    public Segment(Point<T> point1, Point<T> point2)
    {
        Point1 = point1;
        Point2 = point2;
    }
}
