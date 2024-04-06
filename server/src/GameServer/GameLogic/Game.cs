using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using GameServer.Recorder;
using Serilog;

namespace GameServer.GameLogic;

public partial class Game
{
    private readonly GameServer.Recorder.Recorder? _recorder = new(Path.Combine("worlds", "records"));

    #region Fields and properties
    /// <summary>
    /// Gets the config of the game.
    /// </summary>
    public Config Config { get; }
    /// <summary>
    /// Gets the current tick of the game.
    /// </summary>
    /// <remarks>
    /// The first tick is 0.
    /// </remarks>
    public int CurrentTick { get; private set; } = 0;

    private readonly ILogger _logger;

    #endregion


    #region Constructors and finalizers
    /// <summary>
    /// Initializes a new instance of the <see cref="Game"/> class.
    /// </summary>
    public Game(Config config)
    {
        _logger = Log.ForContext("Component", "Game");
        Config = config;

        _map = new Map(config.MapWidth, config.MapHeight, config.SafeZoneMaxRadius, config.SafeZoneTicksUntilDisappear, config.DamageOutsideSafeZone);
        _allPlayers = new List<Player>();

    }

    #endregion


    #region Methods
    public void Initialize()
    {
        SubscribePlayerEvents();
        _map.GenerateMap();

        List<Position> _walls = new();
        List<Recorder.Supplies.suppliesType> _supplies = new();
        for (int i = 0; i < _map._width; i++)
        {
            for (int j = 0; j < _map._height; j++)
            {
                //add wall block into _walls
                if (_map._mapChunk[i, j].IsWall)
                {
                    _walls.Add(new Position(i, j));
                }

                //add supplies into _supplies
                if (_map._mapChunk[i, j].Items.Count > 0)
                {
                    for (int k = 0; k < _map._mapChunk[i, j].Items.Count; k++)
                    {
                        _supplies.Add(new Recorder.Supplies.suppliesType()
                        {
                            name = _map._mapChunk[i, j].Items[k].ItemSpecificName,
                            numb = _map._mapChunk[i, j].Items[k].Count,
                            position = new()
                            {
                                x = i,
                                y = j
                            }
                        });
                    }
                }
            }
        }

        Recorder.Map mapRecord = new()
        {
            Data = new()
            {
                width = _map._width,
                height = _map._height,
                walls = (from wall in _walls
                         select new Recorder.Map.wallsPositionType
                         {
                             x = wall.x,
                             y = wall.y
                         }).ToList()
            }
        };

        _recorder?.Record(mapRecord);

        Recorder.Supplies suppliesRecord = new()
        {
            Data = new()
            {
                supplies = _supplies
            }
        };
    }

    /// <summary>
    /// Ticks the game. This method is called every tick to update the game.
    /// </summary>
    public void Tick()
    {
        try
        {
            lock (this)
            {
                UpdateMap();
                UpdatePlayers();
                UpdateGrenades();
                // AfterGameTickEvent?.Invoke(this, new AfterGameTickEventArgs(this, CurrentTick));

                // Accumulate the current tick at the end of the tick.
                CurrentTick++;
            }

        }
        catch (Exception e)
        {
            _logger.Error($"An exception occurred while ticking the game: {e}");
        }
    }
    # endregion
}
