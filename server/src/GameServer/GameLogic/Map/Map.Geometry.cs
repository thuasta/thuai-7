using GameServer.Engine.Collision;

namespace GameServer.GameLogic;

public partial class Map
{

    //计算两个Position是否连通（无掩体阻挡）（mapChunk[a][b]为1表示(a,b)格子有掩体）
    public bool IsConnected(Position a, Position b)
    {
        int stx = (int)a.x, sty = (int)a.y, edx = (int)b.x, edy = (int)b.y;
        for (int i = stx; i <= edx; i++)
        {
            for (int j = sty; j <= edy; j++)
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
}
