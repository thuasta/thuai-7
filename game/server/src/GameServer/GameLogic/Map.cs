using GameServer.Engine.Collision;

namespace GameServer.GameLogic;

public class Map : IMap
{
    public List<List<IBlock>> MapChunk { get; } = new();
    public ISafeZone SafeZone => throw new NotImplementedException();

    public void GenerateMap()
    {
        GenerateSupplies();
        GenerateBarrier();
    }

    public void GenerateSupplies()
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    private void GenerateBarrier() => throw new NotImplementedException();


    public void GenerateWalls()
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    public void UpdateSupplies()
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    //计算两个Position是否连通（无掩体阻挡）（mapChunk[a][b]为1表示(a,b)格子有掩体）
    public bool IsConnected(Position a, Position b)
    {
        int stx = (int)a.x, sty = (int)a.y, edx = (int)b.x, edy = (int)b.y;
        for (int i = stx; i <= edx; i++)
        {
            for (int j = sty; j <= edy; j++)
            {
                if (MapChunk[i][j].IsWall == true
                && CollisionDetector.checkCross(a, b, i, j))
                {
                    return false;
                }
            }
        }
        return true;
    }
}
