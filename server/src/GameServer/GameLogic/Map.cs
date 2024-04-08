using GameServer.Engine.Collision;

namespace GameServer.GameLogic;

public class Map : IMap
{
    public IBlock[,] MapChunk { get; private set; }
    public int Width { get; }
    public int Height { get; }
    public ISafeZone SafeZone { get; private set; }
    private readonly Random _random = new();
    private readonly List<ObstacleShape> _obstacleShapes;

    private readonly int _tryTimes;
    private readonly int _numSupplyPoints;
    private readonly int _minItemsPerSupply;
    private readonly int _maxItemsPerSupply;

    public Map(int width, int height, float safeZoneMaxRadius, int safeZoneTicksUntilDisappear, int damageOutsideSafeZone)
    {
        Width = width;
        Height = height;
        MapChunk = new IBlock[width, height];

        // Randomly generate the center of the safe zone
        int centerX = (int)_random.NextInt64(0, width);
        int centerY = (int)_random.NextInt64(0, height);
        SafeZone = new SafeZone(new Position(centerX, centerY), safeZoneMaxRadius, safeZoneTicksUntilDisappear, damageOutsideSafeZone);

        _obstacleShapes = new List<ObstacleShape>
        {
            // Define obstacle shapes here, for example:
            new HouseShape(),
            new TreeShape(),
            new RockShape(),
            new TubeShape()
        };

        _tryTimes = Constant.WALL_GENERATE_TRY_TIMES;
        _numSupplyPoints = Constant.NUM_SUPPLY_POINTS;
        _minItemsPerSupply = Constant.MIN_ITEMS_PER_SUPPLY;
        _maxItemsPerSupply = Constant.MAX_ITEMS_PER_SUPPLY;
    }

    public IBlock? GetBlock(int x, int y)
    {
        // Judge if the block is out of the map
        if (x < 0 || x >= MapChunk.GetLength(0) || y < 0 || y >= MapChunk.GetLength(1))
        {
            return null;
        }

        return MapChunk[x, y];
    }
    public IBlock? GetBlock(float x, float y)
    {
        // Convert float to int
        int xInt = (int)x;
        int yInt = (int)y;
        // Judge if the block is out of the map
        if (xInt < 0 || xInt >= MapChunk.GetLength(0) || yInt < 0 || yInt >= MapChunk.GetLength(1))
        {
            return null;
        }

        return MapChunk[xInt, yInt];
    }

    public IBlock? GetBlock(Position position)
    {
        // Convert float to int
        int xInt = (int)position.x;
        int yInt = (int)position.y;
        // Judge if the block is out of the map
        if (xInt < 0 || xInt >= MapChunk.GetLength(0) || yInt < 0 || yInt >= MapChunk.GetLength(1))
        {
            return null;
        }
        return MapChunk[xInt, yInt];
    }
    public void GenerateMap()
    {
        GenerateWalls();
        GenerateSupplies();
    }

    public void GenerateSupplies()
    {
        // Iterate to generate the desired number of supply points
        for (int i = 0; i < _numSupplyPoints; i++)
        {
            // Randomly select position for the supply point
            int x = _random.Next(0, Width);
            int y = _random.Next(0, Height);

            // Check if the selected position is valid (not on an obstacle)
            if (GetBlock(x, y)?.IsWall == false)
            {
                // Randomly determine the number of items for this supply point
                int numItems = _random.Next(_minItemsPerSupply, _maxItemsPerSupply + 1);

                // Generate items and add them to the supply point
                for (int j = 0; j < numItems; j++)
                {
                    // Example: Generate a random item type
                    IItem.ItemKind itemType = (IItem.ItemKind)_random.Next(0, Enum.GetValues(typeof(IItem.ItemKind)).Length);

                    // TODO: According to the item type, randomly sample a specific name for the item
                    string itemSpecificName = WeaponFactory.WeaponNames.ElementAt(_random.Next(0, WeaponFactory.WeaponNames.Length));

                    // Generate a random count for the item (you may customize this part)
                    int itemCount = _random.Next(1, 6); // Random count between 1 and 5

                    // Add the generated item to the supply point
                    AddSupplies(x, y, new Item(itemType, itemSpecificName, itemCount));
                }
            }
        }
    }

    public void GenerateWalls()
    {
        // Clear the map
        Clear();

        // Iterate _tryTimes and place them on the map
        for (int i = 0; i < _tryTimes; i++)
        {
            // Randomly select an obstacle shape
            ObstacleShape shape = _obstacleShapes[_random.Next(0, _obstacleShapes.Count)];

            // Place the obstacle shape on the map
            if (PlaceObstacleShape(shape))
            {
                // Successfully placed the obstacle shape
                // Continue to the next iteration
                continue;
            }
        }
    }

    private bool PlaceObstacleShape(ObstacleShape shape)
    {
        // Randomly select position for the obstacle shape
        int startX = _random.Next(0, Width - shape.MaxWidth);
        int startY = _random.Next(0, Height - shape.MaxHeight);

        // Check if the selected position is valid
        if (IsPositionValid(startX, startY, shape))
        {
            // Place the obstacle shape on the map
            for (int x = 0; x < shape.MaxWidth; x++)
            {
                for (int y = 0; y < shape.MaxHeight; y++)
                {
                    if (shape.IsSolid(x, y))
                    {
                        MapChunk[startX + x, startY + y] = shape.GetBlock(x, y);
                    }
                }
            }

            return true;
        }

        return false;
    }

    private bool IsPositionValid(int startX, int startY, ObstacleShape shape)
    {
        // Check if the position is within the map boundaries
        if (startX < 0 || startY < 0 || startX + shape.MaxWidth >= Width || startY + shape.MaxHeight >= Height)
        {
            return false;
        }

        // Check if the position overlaps with existing obstacles
        for (int x = 0; x < shape.MaxWidth; x++)
        {
            for (int y = 0; y < shape.MaxHeight; y++)
            {
                if (MapChunk[startX + x, startY + y] != null && MapChunk[startX + x, startY + y].IsWall)
                {
                    return false;
                }
            }
        }

        // Check if the position is reachable from the rest of the map
        // Implement connectivity check here (optional)

        return true;
    }


    public void Clear()
    {
        // Clear the map
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                MapChunk[x, y] = new Block(false);
            }
        }
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
                if (MapChunk[i, j].IsWall == true
                && CollisionDetector.checkCross(a, b, i, j))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public abstract class ObstacleShape
    {
        public abstract int MaxWidth { get; }
        public abstract int MaxHeight { get; }

        public abstract bool IsSolid(int x, int y);
        public abstract IBlock GetBlock(int x, int y);
    }

    public class HouseShape : ObstacleShape
    {
        public override int MaxWidth => 5;
        public override int MaxHeight => 5;

        private readonly char[,] _shape =
        {
        {'#', '#', ' ', '#', '#'},
        {'#', ' ', ' ', ' ', '#'},
        {'#', ' ', ' ', ' ', '#'},
        {'#', ' ', ' ', ' ', '#'},
        {'#', '#', ' ', '#', '#'}
    };

        public override bool IsSolid(int x, int y)
        {
            return _shape[x, y] == '#';
        }

        public override IBlock GetBlock(int x, int y)
        {
            // Implement the shape of the house here
            // For simplicity, I'm just using a placeholder
            return new Block(IsSolid(x, y)); // Assuming Wall is a class that implements IBlock
        }
    }

    public class TreeShape : ObstacleShape
    {
        public override int MaxWidth => 2;
        public override int MaxHeight => 2;

        private readonly char[,] _shape = {
            { '#', '#' },
            { '#', '#' }
        };

        public override bool IsSolid(int x, int y)
        {
            return _shape[x, y] == '#';
        }

        public override IBlock GetBlock(int x, int y)
        {
            // Implement the shape of the tree here
            // For simplicity, I'm just using a placeholder
            return new Block(IsSolid(x, y)); // Assuming Wall is a class that implements IBlock
        }
    }

    public class TubeShape : ObstacleShape
    {
        public override int MaxWidth => 3;
        public override int MaxHeight => 6;

        private readonly char[,] _shape = {
            { '#', '#', '#', '#','#', '#' },
            { ' ',' ',' ',' ',' ',' ' },
            { '#', '#', '#', '#','#', '#' }
        };

        public override bool IsSolid(int x, int y)
        {
            return _shape[x, y] == '#';
        }

        public override IBlock GetBlock(int x, int y)
        {
            // Implement the shape of the tree here
            // For simplicity, I'm just using a placeholder
            return new Block(IsSolid(x, y)); // Assuming Wall is a class that implements IBlock
        }
    }

    public class RockShape : ObstacleShape
    {
        public override int MaxWidth => 3;
        public override int MaxHeight => 3;

        private readonly char[,] _shape =
        {
        {' ', '#', ' '},
        {'#', '#', '#'},
        {' ', '#', ' '}
    };

        public override bool IsSolid(int x, int y)
        {
            return _shape[x, y] == '#';
        }

        public override IBlock GetBlock(int x, int y)
        {
            // Implement the shape of the rock here
            // For simplicity, I'm just using a placeholder
            return new Block(IsSolid(x, y)); // Assuming Wall is a class that implements IBlock
        }
    }
}
