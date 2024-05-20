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

    public class LongWallNSShape : ObstacleShape
    {
        public override int MaxWidth => _shape.GetLength(0);
        public override int MaxHeight => _shape.GetLength(1);

        private readonly char[,] _shape;

        public LongWallNSShape(int length)
        {
            _shape = new char[3, length];
            for (int i = 0; i < length; i++)
            {
                if (i == 0 || i == length - 1)
                {
                    _shape[1, i] = ' ';
                }
                else
                {
                    _shape[1, i] = '#';
                }
                _shape[0, i] = ' ';
                _shape[2, i] = ' ';
            }
        }

        public override bool IsSolid(int x, int y)
        {
            return _shape[x, y] == '#';
        }

        public override IBlock GetBlock(int x, int y)
        {
            var random = new Random();
            if (random.NextDouble() < 0.1)
            {
                return new Block(false);
            }
            // Implement the shape of the tree here
            // For simplicity, I'm just using a placeholder
            return new Block(IsSolid(x, y)); // Assuming Wall is a class that implements IBlock
        }
    }

    public class LongWallWEShape : ObstacleShape
    {
        public override int MaxWidth => _shape.GetLength(0);
        public override int MaxHeight => _shape.GetLength(1);

        private readonly char[,] _shape;

        public LongWallWEShape(int length)
        {
            _shape = new char[length, 3];
            for (int i = 0; i < length; i++)
            {
                if (i == 0 || i == length - 1)
                {
                    _shape[i, 1] = ' ';
                }
                else
                {
                    _shape[i, 1] = '#';
                }
                _shape[i, 0] = ' ';
                _shape[i, 2] = ' ';
            }
        }

        public override bool IsSolid(int x, int y)
        {
            return _shape[x, y] == '#';
        }

        public override IBlock GetBlock(int x, int y)
        {
            var random = new Random();

            if (random.NextDouble() < 0.1)
            {
                return new Block(false);
            }

            // Implement the shape of the tree here
            // For simplicity, I'm just using a placeholder
            return new Block(IsSolid(x, y)); // Assuming Wall is a class that implements IBlock
        }
    }


    public class RandomSquareShape : ObstacleShape
    {
        public override int MaxWidth => _shape.GetLength(0);
        public override int MaxHeight => _shape.GetLength(1);

        private readonly char[,] _shape;

        public RandomSquareShape(int length)
        {
            _shape = new char[length, length];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (i == 0 || i == length - 1 || j == 0 || j == length - 1)
                    {
                        _shape[i, j] = ' ';
                    }
                    else
                    {
                        _shape[i, j] = '#';
                    }
                }
            }

            var random = new Random();

            for (int tryCount = 0; tryCount < Math.Pow(length, 0.25) * 10; tryCount++)
            {
                // Position starts from an empty cell.
                Tuple<int, int> currentPosition = new(random.Next(0, length), random.Next(0, length));

                // Random walk until empty and set the path to empty.
                while (currentPosition.Item1 != 0 && currentPosition.Item1 != length - 1 && currentPosition.Item2 != 0 && currentPosition.Item2 != length - 1)
                {
                    _shape[currentPosition.Item1, currentPosition.Item2] = ' ';
                    if (random.Next(0, 2) == 0)
                    {
                        currentPosition = new(currentPosition.Item1 + (random.Next(0, 2) == 0 ? -1 : 1), currentPosition.Item2);
                    }
                    else
                    {
                        currentPosition = new(currentPosition.Item1, currentPosition.Item2 + (random.Next(0, 2) == 0 ? -1 : 1));
                    }
                }
            }
        }

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
