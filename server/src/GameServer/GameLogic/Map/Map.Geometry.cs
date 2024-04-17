using GameServer.Geometry;

namespace GameServer.GameLogic;

public partial class Map
{

    //计算两个Position是否连通（无掩体阻挡）（mapChunk[a][b]为1表示(a,b)格子有掩体）
    public bool IsConnected(Position a, Position b)
    {
        if (GetBlock(a.x, a.y) is null || GetBlock(b.x, b.y) is null)
        {
            return false;
        }

        int stx = (int)a.x, sty = (int)a.y, edx = (int)b.x, edy = (int)b.y;
        for (int i = Math.Min(stx, edx); i <= Math.Max(stx, edx); i++)
        {
            for (int j = Math.Min(sty, edy); j <= Math.Max(sty, edy); j++)
            {
                if (MapChunk[i, j].IsWall == true
                && CollisionDetector.CheckCross(a, b, i, j))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public Position GetRealEndPositon(Position startPosition, Position expectedEndPosition)
    {
        if (GetBlock(startPosition) is null || GetBlock(startPosition)?.IsWall == true)
        {
            return startPosition;
        }

        Position direction = expectedEndPosition - startPosition;
        double ratio = 0.5;
        double delta = 0.25;

        // Binary search to find the real end position
        while (delta > 1e-6)
        {
            Position midPosition = startPosition + direction * ratio;
            if (IsConnected(startPosition, midPosition) == false)
            {
                ratio -= delta;
            }
            else
            {
                ratio += delta;
            }
            delta *= 0.5;
        }

        // Adjust the ratio slightly to avoid the edge case
        if (ratio < 0.01)
        {
            return startPosition;
        }
        else
        {
            ratio *= 0.99;
            return startPosition + direction * ratio;
        }
    }
}
