namespace GameServer.GameLogic;
public partial class Constant
{
    public const int BANDAGE_HEAL = 10;
    public const int WALL_GENERATE_TRY_TIMES = 256;
    public const int FIRST_AID_HEAL = 75;
    public const int TICKS_PER_SECOND = 20;
    //手榴弹爆炸延时的tick数
    public const int GRENADE_EXPLODE_TICK = 100;
    //手雷爆炸的伤害的线性衰减参数
    public const double GRENADE_DAMAGE_DECAY = 13.83;
    //手雷爆炸的伤害的最大值
    public const double GRENADE_MAX_DAMAGE = 108.07;
    //手雷爆炸的伤害的最大半径
    public const double GRENADE_MAX_RADIUS = 6;

    public const int NUM_SUPPLY_POINTS = 1024;

    public const int MIN_ITEMS_PER_SUPPLY = 1;
    public const int MAX_ITEMS_PER_SUPPLY = 64;

    public const int NO_ARMOR_DEFENSE = 0;
    public const int PRIMARY_ARMOR_DEFENSE = 50;

    public const int PREMIUM_ARMOR_DEFENSE = 100;

}
