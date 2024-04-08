using Serilog;

namespace GameServer.GameLogic;

public partial class Game
{
    public event EventHandler<AfterGameTickEventArgs>? AfterGameTickEvent = delegate { };


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

    private readonly object _lock = new();

    private readonly Recorder.Recorder? _recorder = new(Path.Combine("Thuai", "records"));

    #endregion

    #region Constructors and finalizers
    /// <summary>
    /// Initializes a new instance of the <see cref="Game"/> class.
    /// </summary>
    public Game(Config config)
    {
        _logger = Log.ForContext("Component", "Game");
        Config = config;

        GameMap = new Map(config.MapWidth, config.MapHeight, config.SafeZoneMaxRadius, config.SafeZoneTicksUntilDisappear, config.DamageOutsideSafeZone);
        AllPlayers = new List<Player>();

    }

    #endregion

    #region Methods
    public void SaveRecord()
    {
        _recorder?.Save();
    }
    /// <summary>
    /// Initializes the game.
    /// </summary>
    public void Initialize()
    {
        foreach (Player player in AllPlayers)
        {
            SubscribePlayerEvents(player);
        }
        GameMap.GenerateMap();

        List<Position> walls = new();
        List<Recorder.Supplies.suppliesType> supplies = new();
        for (int i = 0; i < GameMap.Width; i++)
        {
            for (int j = 0; j < GameMap.Height; j++)
            {
                //add wall block into walls
                if (GameMap.MapChunk[i, j].IsWall)
                {
                    walls.Add(new Position(i, j));
                }

                //add supplies into supplies
                if (GameMap.MapChunk[i, j].Items.Count > 0)
                {
                    for (int k = 0; k < GameMap.MapChunk[i, j].Items.Count; k++)
                    {
                        supplies.Add(new Recorder.Supplies.suppliesType()
                        {
                            name = GameMap.MapChunk[i, j].Items[k].ItemSpecificName,
                            numb = GameMap.MapChunk[i, j].Items[k].Count,
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
                width = GameMap.Width,
                height = GameMap.Height,
                walls = (from wall in walls
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
                supplies = supplies
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
            lock (_lock)
            {
                CurrentTick++;

                UpdateMap();
                UpdatePlayers();
                UpdateGrenades();

                Recorder.CompetitionUpdate competitionUpdateRecord = new()
                {
                    currentTicks = CurrentTick,
                    Data = new()
                    {
                        players = (from player in AllPlayers
                                   select new Recorder.CompetitionUpdate.playersType
                                   {
                                       playerId = player.PlayerId,
                                       armor = player.PlayerArmor.ItemSpecificName,
                                       position = new()
                                       {
                                           x = player.PlayerPosition.x,
                                           y = player.PlayerPosition.y
                                       },
                                       health = player.Health,
                                       speed = player.Speed,
                                       firearm = new()
                                       {
                                           name = player.PlayerWeapon.ItemSpecificName,
                                           distance = player.PlayerWeapon.Range
                                       },
                                       inventory = (from supplies in player.PlayerBackPack.Items
                                                    select new Recorder.CompetitionUpdate.inventoryType
                                                    {
                                                        name = supplies.ItemSpecificName,
                                                        numb = supplies.Count
                                                    }).ToList()
                                   }).ToList(),
                        events = _events
                    }
                };

                _recorder?.Record(competitionUpdateRecord);

                _events.Clear();
                // Dereference of a possibly null reference.
                // AfterGameTickEvent?.Invoke(this, new AfterGameTickEventArgs(this, CurrentTick));

                AfterGameTickEvent?.Invoke(this, new AfterGameTickEventArgs(this, CurrentTick));
            }

        }
        catch (Exception e)
        {
            _logger.Error($"An exception occurred while ticking the game: {e}");
        }
    }
    # endregion
}
