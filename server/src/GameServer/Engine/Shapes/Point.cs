namespace GameServer.Engine.Shapes;

public struct Point<T>
{
    public T X { get; set; }
    public T Y { get; set; }

    public Point(T x, T y)
    {
        X = x;
        Y = y;
    }
}
