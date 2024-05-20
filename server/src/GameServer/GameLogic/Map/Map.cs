using GameServer.Geometry;
using Serilog;

namespace GameServer.GameLogic;

public partial class Map
{
    public IBlock[,] MapChunk { get; private set; }
    public int Width { get; }
    public int Height { get; }
    public ISafeZone SafeZone { get; private set; }
    private readonly Random _random = new();
    private readonly List<ObstacleShape> _obstacleShapes;
    private readonly List<ObstacleShape> _longWallShapes;
    private readonly List<ObstacleShape> _randomSquareShapes;

    private readonly int _tryTimes;
    private readonly int _numSupplyPoints;
    private readonly int _minItemsPerSupply;
    private readonly int _maxItemsPerSupply;

    private readonly ILogger _logger = Log.ForContext("Component", "Map");

    public Map(int width, int height, float safeZoneMaxRadius, int safeZoneTicksUntilDisappear, int damageOutsideSafeZone)
    {
        Width = width;
        Height = height;
        MapChunk = new IBlock[width, height];

        // Randomly generate the center of the safe zone
        int centerX = 128;
        int centerY = 128;
        SafeZone = new SafeZone(new Position(centerX, centerY), safeZoneMaxRadius, safeZoneTicksUntilDisappear, damageOutsideSafeZone);

        _obstacleShapes =
        [
            // Define obstacle shapes here, for example:
            new HouseShape(),
            new TreeShape(),
            new RockShape(),
            new TubeShape(),
            new PoleShape(),
            new CottageNShape(),
            new CottageEShape(),
            new CottageSShape(),
            new CottageWShape(),
            new WallEWShape(),
            new WallNSShape(),
            new MazeShape(),
            new Corner1Shape(),
            new Corner2Shape(),
            new Corner3Shape(),
            new Corner4Shape(),
            new CarShape(),
        ];

        _longWallShapes =
        [
            new LongWallNSShape(64),
            new LongWallNSShape(128),
            new LongWallNSShape(256),
            new LongWallWEShape(64),
            new LongWallWEShape(128),
            new LongWallWEShape(256),
        ];

        _randomSquareShapes =
        [
            new RandomSquareShape(32),
            new RandomSquareShape(64),
            new RandomSquareShape(128),
            new RandomSquareShape(256),
            new RandomSquareShape(32),
            new RandomSquareShape(64),
            new RandomSquareShape(128),
            new RandomSquareShape(256),
            new RandomSquareShape(32),
            new RandomSquareShape(64),
            new RandomSquareShape(128),
            new RandomSquareShape(256),
            new RandomSquareShape(32),
            new RandomSquareShape(64),
            new RandomSquareShape(128),
            new RandomSquareShape(256),
            new RandomSquareShape(32),
            new RandomSquareShape(64),
            new RandomSquareShape(128),
            new RandomSquareShape(256),
            new RandomSquareShape(32),
            new RandomSquareShape(64),
            new RandomSquareShape(128),
            new RandomSquareShape(256),
            new RandomSquareShape(32),
            new RandomSquareShape(64),
            new RandomSquareShape(128),
            new RandomSquareShape(256),
            new RandomSquareShape(32),
            new RandomSquareShape(64),
            new RandomSquareShape(128),
            new RandomSquareShape(256),
            new RandomSquareShape(32),
            new RandomSquareShape(64),
            new RandomSquareShape(128),
            new RandomSquareShape(256),
            new RandomSquareShape(32),
            new RandomSquareShape(64),
            new RandomSquareShape(128),
            new RandomSquareShape(256),
        ];

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
        if (x < 0 || x >= MapChunk.GetLength(0) || y < 0 || y >= MapChunk.GetLength(1))
        {
            return null;
        }

        int xInt = (int)x;
        int yInt = (int)y;
        return MapChunk[xInt, yInt];
    }
    public IBlock? GetBlock(double x, double y)
    {
        if (x < 0 || x >= MapChunk.GetLength(0) || y < 0 || y >= MapChunk.GetLength(1))
        {
            return null;
        }

        int xInt = (int)x;
        int yInt = (int)y;
        return MapChunk[xInt, yInt];
    }

    public IBlock? GetBlock(Position position)
    {
        return GetBlock(position.x, position.y);
    }

    public Position GenerateValidPosition()
    {
        // Randomly generate a position
        int x = _random.Next(0, Width);
        int y = _random.Next(0, Height);

        // Check if the position is valid
        while (GetBlock(x, y) is null || GetBlock(x, y)?.IsWall == true)
        {
            x = _random.Next(0, Width);
            y = _random.Next(0, Height);
        }

        return new Position(x, y);
    }

    public void AddSupplies(int x, int y, IItem item)
    {
        IBlock? block = GetBlock(x, y);

        block?.GenerateItems(item);
    }

    public void RemoveSupplies(int x, int y, IItem item)
    {
        IBlock? block = GetBlock(x, y);

        block?.RemoveItems(item);
    }
}
