using GameServer.Geometry;

namespace GameServer.GameLogic;

public partial class Map
{
    public bool IsConnected(Position a, Position b)
    {
        if (GetBlock(a.x, a.y) is null || GetBlock(a.x, a.y)?.IsWall == true)
        {
            return false;
        }
        if (GetBlock(b.x, b.y) is null || GetBlock(b.x, b.y)?.IsWall == true)
        {
            return false;
        }

        // a and b is not in wall

        // a and b is at the same block
        if ((int)a.x == (int)b.x && (int)a.y == (int)b.y)
        {
            return true;
        }

        Position direction = (b - a).Normalize();
        if (direction == new Position(0, 0))
        {
            // a and b is the same position
            return true;
        }

        int deltaX = (direction.x > 0) ? 0 : -1;
        int deltaY = (direction.y > 0) ? 0 : -1;
        double offset = 0.5;

        // Check x axis
        if ((int)a.x != (int)b.x)
        {
            double k = direction.y / direction.x;
            foreach (double x in GetXaxisBlockIndex(a.x, b.x))
            {
                double y = k * (x - a.x) + a.y;
                if (GetBlock(x + offset + deltaX, y) is null || GetBlock(x + offset + deltaX, y)?.IsWall == true)
                {
                    return false;
                }
            }
        }
        // Check y axis
        if ((int)a.y != (int)b.y)
        {
            double k = direction.x / direction.y;
            foreach (double y in GetYaxisBlockIndex(a.y, b.y))
            {
                double x = k * (y - a.y) + a.x;
                if (GetBlock(x, y + offset + deltaY) is null || GetBlock(x, y + offset + deltaY)?.IsWall == true)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public Position GetRealEndPositon(
        Position startPosition, Position expectedEndPosition, bool independentAxisCauculating = false
    )
    {
        if (GetBlock(startPosition) is null || GetBlock(startPosition)?.IsWall == true)
        {
            return new(startPosition.x, startPosition.y);
        }
        if (IsConnected(startPosition, expectedEndPosition))
        {
            return new(expectedEndPosition.x, expectedEndPosition.y);
        }

        // Start position and expected end position is not connected
        double distance = (expectedEndPosition - startPosition).Length();
        double diseps = Constant.DISTANCE_ERROR;
        if (distance < diseps)
        {
            return new(startPosition.x, startPosition.y);
        }

        Position direction = (expectedEndPosition - startPosition).Normalize();
        if (direction == new Position(0, 0))
        {
            return new(startPosition.x, startPosition.y);
        }

        if (independentAxisCauculating == false)
        {
            Position a = new(startPosition.x, startPosition.y);
            Position b = new(expectedEndPosition.x, expectedEndPosition.y);
            Position firstHitX = new(b.x, b.y);
            Position firstHitY = new(b.x, b.y);

            int deltaX = (direction.x > 0) ? 0 : -1;
            int deltaY = (direction.y > 0) ? 0 : -1;
            double offset = 0.5;

            // Check x axis
            if ((int)a.x != (int)b.x)
            {
                double k = direction.y / direction.x;
                foreach (double x in GetXaxisBlockIndex(a.x, b.x))
                {
                    double y = k * (x - a.x) + a.y;
                    if (GetBlock(x + offset + deltaX, y) is null || GetBlock(x + offset + deltaX, y)?.IsWall == true)
                    {
                        firstHitX = new Position(x, y) - direction * diseps;
                        break;
                    }
                }
            }
            // Check y axis
            if ((int)a.y != (int)b.y)
            {
                double k = direction.x / direction.y;
                foreach (double y in GetYaxisBlockIndex(a.y, b.y))
                {
                    double x = k * (y - a.y) + a.x;
                    if (GetBlock(x, y + offset + deltaY) is null || GetBlock(x, y + offset + deltaY)?.IsWall == true)
                    {
                        firstHitY = new Position(x, y) - direction * diseps;
                        break;
                    }
                }
            }

            if (Position.Distance(firstHitX, a) < Position.Distance(firstHitY, a))
            {
                return new(firstHitX.x, firstHitX.y);
            }
            else
            {
                return new(firstHitY.x, firstHitY.y);
            }
        }
        else
        {
            Position hitAt = GetRealEndPositon(startPosition, expectedEndPosition, false);
            // Try moving along y axis
            Position result = GetRealEndPositon(hitAt, new(hitAt.x, expectedEndPosition.y), false);
            if (Position.Distance(result, hitAt) < diseps)
            {
                // Try moving along x axis
                result = GetRealEndPositon(hitAt, new(expectedEndPosition.x, hitAt.y), false);
            }
            return new(result.x, result.y);
        }
    }

    private List<int> GetXaxisBlockIndex(double start, double end)
    {
        bool reversed = false;
        if (start > end)
        {
            (end, start) = (start, end);
            reversed = true;
        }
        List<int> result = new();
        int startX = (int)Math.Ceiling(start);
        int endX = (int)Math.Floor(end);

        // Result is empty if startX > endX
        for (int i = startX; i <= endX; i++)
        {
            result.Add(i);
        }
        if (reversed)
        {
            result.Reverse();
        }
        return new(result);
    }

    private List<int> GetYaxisBlockIndex(double start, double end)
    {
        bool reversed = false;
        if (start > end)
        {
            (end, start) = (start, end);
            reversed = true;
        }
        List<int> result = new();
        int startY = (int)Math.Ceiling(start);
        int endY = (int)Math.Floor(end);

        // Result is empty if startY > endY
        for (int i = startY; i <= endY; i++)
        {
            result.Add(i);
        }
        if (reversed)
        {
            result.Reverse();
        }
        return new(result);
    }
}
