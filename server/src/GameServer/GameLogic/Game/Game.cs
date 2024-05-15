using GameServer.Geometry;
using Serilog;

namespace GameServer.GameLogic;

public partial class Game
{
    public event EventHandler<AfterGameInitializationEventArgs>? AfterGameInitializationEvent = delegate { };
    public event EventHandler<AfterGameTickEventArgs>? AfterGameTickEvent = delegate { };
    public event EventHandler<AfterGameFinishEventArgs>? AfterGameFinishEvent = delegate { };

    public enum GameStage
    {
        Waiting,
        Preparing,
        Fighting,
        Finished
    }

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

    public GameStage Stage { get; private set; } = GameStage.Waiting;

    private readonly ILogger _logger;

    private readonly object _lock = new();

    private readonly Recorder.Recorder? _recorder = new(
        "Records",
        "record.dat",
        "result.json"
    );

    #endregion

    #region Constructors and finalizers
    /// <summary>
    /// Initializes a new instance of the <see cref="Game"/> class.
    /// </summary>
    public Game(Config config)
    {
        _logger = Log.ForContext("Component", "Game");
        Config = config;

        GameMap = new Map(config.MapLength, config.MapLength, config.SafeZoneInitialRadius, config.SafeZoneShrinkTime, config.DamagePerTickOutsideSafeZone);
        AllPlayers = new List<Player>();

    }

    #endregion

    #region Methods
    public void SaveRecord()
    {
        _recorder?.Save();
    }

    public void SaveResults(Result result)
    {
        _recorder?.SaveResults(result);
    }

    /// <summary>
    /// Initializes the game.
    /// </summary>
    public void Initialize()
    {
        try
        {
            if (AllPlayers.Count <= 0)
            {
                _logger.Warning("No player is in the game.");
            }

            lock (_lock)
            {
                GameMap.GenerateMap();

                Stage = GameStage.Preparing;

                // Randomly choose the position of each player
                foreach (Player player in AllPlayers)
                {
                    player.PlayerPosition = GameMap.GenerateValidPosition() + new Position(0.5, 0.5);
                }

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
                        walls = (
                            from wall in walls
                            select new Recorder.Map.wallsPositionType
                            {
                                x = wall.x,
                                y = wall.y
                            }
                        ).ToList()
                    }
                };

                _recorder?.Record(mapRecord);

                Recorder.Supplies suppliesRecord = new()
                {
                    Data = new()
                    {
                        supplies = new(supplies)
                    }
                };

                _recorder?.Record(suppliesRecord);

                AfterGameInitializationEvent?.Invoke(this, new AfterGameInitializationEventArgs(this));
            }
        }
        catch (Exception e)
        {
            _logger.Error($"Failed to initialize the game: {e.Message}");
            _logger.Debug($"{e}");
        }
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
                if (Stage == GameStage.Waiting)
                {
                    _logger.Error("The game should be initialized before ticking.");
                    return;
                }
                if (Stage == GameStage.Finished)
                {
                    _logger.Warning("The game has already finished. No more ticks will be processed.");
                    return;
                }

                CurrentTick++;
                if (CurrentTick > Constant.PREPERATION_TICKS)
                {
                    Stage = GameStage.Fighting;
                }

                _logger.Debug($"Current tick: {CurrentTick}");

                int alivePlayers = 0;
                foreach (Player player in AllPlayers)
                {
                    if (player.IsAlive == true)
                    {
                        alivePlayers++;
                    }
                }
                if (alivePlayers <= 1)
                {
                    Stage = GameStage.Finished;
                    AfterGameFinishEvent?.Invoke(this, new AfterGameFinishEventArgs());
                }

                if (Stage == GameStage.Fighting)
                {
                    UpdateMap();
                    UpdatePlayers();
                    UpdateGrenades();
                }

                Recorder.CompetitionUpdate competitionUpdateRecord = new()
                {
                    currentTicks = CurrentTick,
                    Data = new()
                    {
                        players = (
                            from player in AllPlayers
                            select new Recorder.CompetitionUpdate.playersType
                            {
                                playerId = player.PlayerId,
                                token = player.Token,
                                armor = player.PlayerArmor.ItemSpecificName,
                                currentArmorHealth = player.PlayerArmor.Health,
                                position = new()
                                {
                                    x = player.PlayerPosition.x,
                                    y = player.PlayerPosition.y
                                },
                                health = player.Health,
                                speed = player.Speed,
                                firearm = new()
                                {
                                    name = player.PlayerWeapon.Name,
                                    distance = player.PlayerWeapon.Range
                                },
                                firearmsPool = (from weapon in player.WeaponSlot
                                                select new Recorder.CompetitionUpdate.firearmType
                                                {
                                                    name = weapon.Name,
                                                    distance = weapon.Range
                                                }).ToList(),
                                inventory = (from supplies in player.PlayerBackPack.Items
                                             select new Recorder.CompetitionUpdate.inventoryType
                                             {
                                                 name = supplies.ItemSpecificName,
                                                 numb = supplies.Count
                                             }).ToList()
                            }
                        ).ToList(),
                        events = new(_events)
                    }
                };

                _recorder?.Record(competitionUpdateRecord);

                _events.Clear();
                AfterGameTickEvent?.Invoke(
                    this, new AfterGameTickEventArgs(AllPlayers, GameMap, CurrentTick, _allGrenades)
                );
            }
        }
        catch (Exception e)
        {
            _logger.Error($"An exception occurred while ticking the game: {e.Message}");
            _logger.Debug($"{e}");
        }
    }

    /// <summary>
    /// Judges the game.
    /// </summary>
    /// <returns>Winner's player id.</returns>
    public int Judge()
    {
        if (Stage != GameStage.Finished)
        {
            _logger.Error("The game should be finished before judging.");
        }

        _logger.Information("Judging the game.");

        Player lastSurvivor = AllPlayers[0];
        if (lastSurvivor.DieTime is not null)
        {
            foreach (Player player in AllPlayers)
            {
                if (player.DieTime is null)
                {
                    lastSurvivor = player;
                    break;
                }

                if (player.DieTime is not null && player.DieTime > lastSurvivor.DieTime)
                {
                    lastSurvivor = player;
                }
            }
        }

        _logger.Information($"The winner is Player {lastSurvivor.PlayerId}.");

        return lastSurvivor.PlayerId;
    }
    #endregion
}
