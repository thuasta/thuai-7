namespace GameServer.GameLogic
{
    public class Position
    {
        public double x;
        public double y;

        public Position(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public static double Distance(Position a, Position b)
        {
            return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
        }
        // Operator overloading
        public static Position operator +(Position a, Position b)
        {
            return new Position(a.x + b.x, a.y + b.y);
        }
        public static Position operator -(Position a, Position b)
        {
            return new Position(a.x - b.x, a.y - b.y);
        }
        public static Position operator *(Position a, double b)
        {
            return new Position(a.x * b, a.y * b);
        }
        public static Position operator /(Position a, double b)
        {
            return new Position(a.x / b, a.y / b);
        }
        // Normalize the vector
        public Position Normalize()
        {
            double length = Math.Sqrt(x * x + y * y);
            return new Position(x / length, y / length);
        }
        public double Length()
        {
            return Math.Sqrt(LengthSquared());
        }
        public override string ToString()
        {
            return $"({x}, {y})";
        }
        public double LengthSquared()
        {
            return x * x + y * y;
        }
        public static double Dot(Position a, Position b)
        {
            return a.x * b.x + a.y * b.y;
        }
    }
}
