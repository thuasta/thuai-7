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

        Position direction = (b - a).Normalize();
        double distance = Position.Distance(b, a);

        double step = 0.02;
        Position currentPosition = new(a.x, a.y);
        while (distance > 0)
        {
            if (distance < step)
            {
                return true;
            }
            currentPosition += direction * step;
            distance -= step;
            if (GetBlock(currentPosition) is null || GetBlock(currentPosition)?.IsWall == true)
            {
                return false;
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

        Position direction = (expectedEndPosition - startPosition).Normalize();
        double distance = Position.Distance(startPosition, expectedEndPosition);

        double step = 0.02;
        Position currentPosition = new(startPosition.x, startPosition.y);
        while (distance > 0)
        {
            if (distance < step)
            {
                return currentPosition;
            }
            currentPosition += direction * step;
            distance -= step;
            if (GetBlock(currentPosition) is null || GetBlock(currentPosition)?.IsWall == true)
            {
                return currentPosition - direction * step;
            }
        }
        return currentPosition;
    }
}
