using GameServer.GameLogic;
using GameServer.Geometry.Shapes;

namespace GameServer.Geometry;

public class CollisionDetector
{
    public static double CrossProduct(Position a, Position b, Position c)
    {
        return (b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y);
    }

    public static bool AreOnSameSide(Position a, Position b, Position[] positions)
    {
        double abCrossProduct = CrossProduct(a, b, positions[0]);
        for (int i = 1; i < positions.Length; i++)
        {
            double currentCrossProduct = CrossProduct(a, b, positions[i]);
            if (currentCrossProduct * abCrossProduct < 0)
            {
                return false; // Points are not on the same side
            }
        }
        return true; // All points are on the same side
    }

    public static bool CheckCross(Position a, Position b, int i, int j)
    {
        Position[] positions = new Position[]
        {
            new Position(i, j),
            new Position(i, j + 1),
            new Position(i + 1, j),
            new Position(i + 1, j + 1)
        };
        if (!AreOnSameSide(a, b, positions))
        {
            return false;
        }
        return true;
    }

    public static bool IsCrossing(Segment segment, Circle circle)
    {
        Position a = segment.Start;
        Position b = segment.End;
        Position c = circle.Center;

        Position vectorAB = b - a;
        Position vectorAC = c - a;

        double ac = Position.Distance(a, c);
        double bc = Position.Distance(b, c);

        if (a == b)
        {
            return ac <= circle.Radius;
        }
        if (ac <= circle.Radius || bc <= circle.Radius)
        {
            return true;
        }

        // vectorAB is non-zero and vectorAC is non-zero
        double cosTheta = Position.Dot(vectorAB, vectorAC) / (vectorAB.Length() * vectorAC.Length());
        double sinTheta = Math.Sqrt(1 - cosTheta * cosTheta);
        double distance = vectorAC.Length() * sinTheta;
        Position vectorAConAB = vectorAB.Normalize() * (Position.Dot(vectorAC, vectorAB) / vectorAB.Length());

        if (distance > circle.Radius)
        {
            return false;
        }
        else
        {
            if (vectorAConAB.Length() > vectorAB.Length() || Position.Dot(vectorAConAB, vectorAB) <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
