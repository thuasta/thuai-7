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
        public override int MaxWidth => 5 + 2;
        public override int MaxHeight => 5 + 2;

        private readonly char[,] _shape =
        {
            {' ', ' ', ' ', ' ', ' ', ' ', ' '},
            {' ', '#', '#', ' ', '#', '#', ' '},
            {' ', '#', ' ', ' ', ' ', '#', ' '},
            {' ', '#', ' ', ' ', ' ', '#', ' '},
            {' ', '#', ' ', ' ', ' ', '#', ' '},
            {' ', '#', '#', ' ', '#', '#', ' '},
            {' ', ' ', ' ', ' ', ' ', ' ', ' '}
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
        public override int MaxWidth => 2 + 2;
        public override int MaxHeight => 2 + 2;

        private readonly char[,] _shape = {
            {' ', ' ', ' ', ' '},
            {' ', '#', '#', ' '},
            {' ', '#', '#', ' '},
            {' ', ' ', ' ', ' '}
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
        public override int MaxWidth => 3 + 2;
        public override int MaxHeight => 6 + 2;

        private readonly char[,] _shape = {
            {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '},
            {' ', '#', '#', '#', '#', '#', '#', ' '},
            {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '},
            {' ', '#', '#', '#', '#', '#', '#', ' '},
            {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '}
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
        public override int MaxWidth => 3 + 2;
        public override int MaxHeight => 3 + 2;

        private readonly char[,] _shape =
        {
            {' ', ' ', ' ', ' ', ' '},
            {' ', ' ', '#', ' ', ' '},
            {' ', '#', '#', '#', ' '},
            {' ', ' ', '#', ' ', ' '},
            {' ', ' ', ' ', ' ', ' '}
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
    /* The following obstacles are added on 2024.4.17
   Contents -------------------------------------
   Pole ( Telegraph Pole )
   Cottage ( With door on N/S/E/W side )
   Wall ( N-S & E-W)
   Maze
   Corner ( N/S/E/W )
   Car
   ----------------------------------------------*/
    public class PoleShape : ObstacleShape
    {
        public override int MaxWidth => 1 + 2;
        public override int MaxHeight => 1 + 2;

        private readonly char[,] _shape =
        {
            {' ', ' ', ' '},
            {' ', '#', ' '},
            {' ', ' ', ' '}
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
    public class CottageNShape : ObstacleShape
    {
        public override int MaxWidth => 5 + 2;
        public override int MaxHeight => 5 + 2;

        private readonly char[,] _shape =
        {
            {' ', ' ', ' ', ' ', ' ', ' ', ' '},
            {' ', '#', '#', ' ', '#', '#', ' '},
            {' ', '#', ' ', ' ', ' ', '#', ' '},
            {' ', '#', ' ', ' ', ' ', '#', ' '},
            {' ', '#', ' ', ' ', ' ', '#', ' '},
            {' ', '#', '#', '#', '#', '#', ' '},
            {' ', ' ', ' ', ' ', ' ', ' ', ' '}
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
    public class CottageSShape : ObstacleShape
    {
        public override int MaxWidth => 5 + 2;
        public override int MaxHeight => 5 + 2;

        private readonly char[,] _shape =
        {
            {' ', ' ', ' ', ' ', ' ', ' ', ' '},
            {' ', '#', '#', '#', '#', '#', ' '},
            {' ', '#', ' ', ' ', ' ', '#', ' '},
            {' ', '#', ' ', ' ', ' ', '#', ' '},
            {' ', '#', ' ', ' ', ' ', '#', ' '},
            {' ', '#', '#', ' ', '#', '#', ' '},
            {' ', ' ', ' ', ' ', ' ', ' ', ' '}
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
    public class CottageEShape : ObstacleShape
    {
        public override int MaxWidth => 5 + 2;
        public override int MaxHeight => 5 + 2;

        private readonly char[,] _shape =
        {
            {' ', ' ', ' ', ' ', ' ', ' ', ' '},
            {' ', '#', '#', '#', '#', '#', ' '},
            {' ', '#', ' ', ' ', ' ', '#', ' '},
            {' ', '#', ' ', ' ', ' ', ' ', ' '},
            {' ', '#', ' ', ' ', ' ', '#', ' '},
            {' ', '#', '#', '#', '#', '#', ' '},
            {' ', ' ', ' ', ' ', ' ', ' ', ' '}
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
    public class CottageWShape : ObstacleShape
    {
        public override int MaxWidth => 5 + 2;
        public override int MaxHeight => 5 + 2;

        private readonly char[,] _shape =
        {
            {' ', ' ', ' ', ' ', ' ', ' ', ' '},
            {' ', '#', '#', '#', '#', '#', ' '},
            {' ', '#', ' ', ' ', ' ', '#', ' '},
            {' ', ' ', ' ', ' ', ' ', '#', ' '},
            {' ', '#', ' ', ' ', ' ', '#', ' '},
            {' ', '#', '#', '#', '#', '#', ' '},
            {' ', ' ', ' ', ' ', ' ', ' ', ' '}
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
    public class WallEWShape : ObstacleShape
    {
        public override int MaxWidth => 1 + 2;
        public override int MaxHeight => 5 + 2;

        private readonly char[,] _shape = {
            {' ', ' ', ' ', ' ', ' ', ' ', ' '},
            {' ', '#', '#', '#', '#', '#', ' '},
            {' ', ' ', ' ', ' ', ' ', ' ', ' '}
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
    public class WallNSShape : ObstacleShape
    {
        public override int MaxWidth => 5 + 2;
        public override int MaxHeight => 1 + 2;

        private readonly char[,] _shape = {
            {' ', ' ', ' '},
            {' ', '#', ' '},
            {' ', '#', ' '},
            {' ', '#', ' '},
            {' ', '#', ' '},
            {' ', '#', ' '},
            {' ', ' ', ' '}
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
    public class MazeShape : ObstacleShape
    {
        public override int MaxWidth => 5 + 2;
        public override int MaxHeight => 10 + 2;

        private readonly char[,] _shape =
        {
            {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '},
            {' ', '#', '#', '#', '#', ' ', '#', '#', '#', '#', '#', ' '},
            {' ', ' ', ' ', '#', ' ', ' ', '#', ' ', ' ', ' ', '#', ' '},
            {' ', '#', ' ', '#', ' ', '#', ' ', ' ', '#', ' ', '#', ' '},
            {' ', '#', ' ', ' ', ' ', '#', ' ', '#', '#', ' ', ' ', ' '},
            {' ', '#', '#', '#', '#', '#', ' ', '#', '#', '#', '#', ' '},
            {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '}
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
    public class Corner1Shape : ObstacleShape
    {
        public override int MaxWidth => 2 + 2;
        public override int MaxHeight => 2 + 2;

        private readonly char[,] _shape =
        {
            {' ', ' ', ' ', ' '},
            {' ', ' ', '#', ' '},
            {' ', '#', '#', ' '},
            {' ', ' ', ' ', ' '}
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
    public class Corner2Shape : ObstacleShape
    {
        public override int MaxWidth => 2 + 2;
        public override int MaxHeight => 2 + 2;

        private readonly char[,] _shape =
        {
            {' ', ' ', ' ', ' '},
            {' ', '#', ' ', ' '},
            {' ', '#', '#', ' '},
            {' ', ' ', ' ', ' '}
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
    public class Corner3Shape : ObstacleShape
    {
        public override int MaxWidth => 2 + 2;
        public override int MaxHeight => 2 + 2;

        private readonly char[,] _shape =
        {
            {' ', ' ', ' ', ' '},
            {' ', '#', '#', ' '},
            {' ', '#', ' ', ' '},
            {' ', ' ', ' ', ' '}
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
    public class Corner4Shape : ObstacleShape
    {
        public override int MaxWidth => 2 + 2;
        public override int MaxHeight => 2 + 2;

        private readonly char[,] _shape =
        {
            {' ', ' ', ' ', ' '},
            {' ', '#', '#', ' '},
            {' ', ' ', '#', ' '},
            {' ', ' ', ' ', ' '}
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
    public class CarShape : ObstacleShape
    {
        public override int MaxWidth => 2 + 2;
        public override int MaxHeight => 3 + 2;

        private readonly char[,] _shape = {
            {' ', ' ', ' ', ' ', ' '},
            {' ', '#', '#', '#', ' '},
            {' ', '#', '#', '#', ' '},
            {' ', ' ', ' ', ' ', ' '}
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
}
