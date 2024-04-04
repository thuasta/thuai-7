using GameServer.Engine.Collision;

namespace GameServer.GameLogic;

public class Map : IMap
{
    private readonly IBlock[,] _mapChunk;
    private readonly int _width;
    private readonly int _height;
    public ISafeZone SafeZone => throw new NotImplementedException();
    public Map(int width, int height)
    {
        _width = width;
        _height = height;
        _mapChunk = new IBlock[width, height];
    }

    public IBlock? GetBlock(int x, int y)
    {
        // Judge if the block is out of the map
        if (x < 0 || x >= _mapChunk.GetLength(0) || y < 0 || y >= _mapChunk.GetLength(1))
        {
            return null;
        }

        return _mapChunk[x, y];
    }
    public IBlock? GetBlock(float x, float y)
    {
        // Convert float to int
        int xInt = (int)x;
        int yInt = (int)y;
        // Judge if the block is out of the map
        if (xInt < 0 || xInt >= _mapChunk.GetLength(0) || yInt < 0 || yInt >= _mapChunk.GetLength(1))
        {
            return null;
        }

        return _mapChunk[xInt, yInt];
    }

    public IBlock? GetBlock(Position position)
    {
        // Convert float to int
        int xInt = (int)position.x;
        int yInt = (int)position.y;
        // Judge if the block is out of the map
        if (xInt < 0 || xInt >= _mapChunk.GetLength(0) || yInt < 0 || yInt >= _mapChunk.GetLength(1))
        {
            return null;
        }
        return _mapChunk[xInt, yInt];

    }
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
    public void Clear()
    {
        // TODO
        throw new NotImplementedException();
    }
    public void AddSupplies(int x, int y, IItem item)
    {
        // TODO: Implement
        IBlock? block = GetBlock(x, y);

        block?.GenerateItems(item.Kind, item.ItemSpecificName, item.Count);
    }
    public void RemoveSupplies(int x, int y, IItem item)
    {
        // TODO: Implement
        IBlock? block = GetBlock(x, y);

        block?.RemoveItems(item.Kind, item.ItemSpecificName, item.Count);
    }
    //计算两个Position是否连通（无掩体阻挡）（mapChunk[a][b]为1表示(a,b)格子有掩体）
    public bool IsConnected(Position a, Position b)
    {
        int stx = (int)a.x, sty = (int)a.y, edx = (int)b.x, edy = (int)b.y;
        for (int i = stx; i <= edx; i++)
        {
            for (int j = sty; j <= edy; j++)
            {
                if (_mapChunk[i, j].IsWall == true
                && CollisionDetector.checkCross(a, b, i, j))
                {
                    return false;
                }
            }
        }
        return true;
    }
}
