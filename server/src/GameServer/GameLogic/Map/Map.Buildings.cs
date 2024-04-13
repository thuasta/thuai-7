namespace GameServer.GameLogic;

public partial class Map
{

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
