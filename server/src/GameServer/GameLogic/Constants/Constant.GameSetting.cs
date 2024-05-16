namespace GameServer.GameLogic;

public partial class Constant
{
    public const int TICKS_PER_SECOND = 20;

    public const int PREPERATION_TICKS = 10 * TICKS_PER_SECOND;

    public const int WALL_GENERATE_TRY_TIMES = 2048;

    public const int NUM_SUPPLY_POINTS = 256;
    public const int MIN_ITEMS_PER_SUPPLY = 1;
    public const int MAX_ITEMS_PER_SUPPLY = 4;

    public const double DISTANCE_ERROR = 1e-3;
    public const double CALCULATION_ERROR = 1e-6;
    public const double BOUNCE_DISTANCE = 0.01;
}
