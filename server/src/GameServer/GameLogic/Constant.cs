namespace GameServer.GameLogic;
public partial class Constant
{
    public const int TICKS_PER_SECOND = 20;
    //手榴弹爆炸延时的tick数
    public const int GRENADE_EXPLODE_TICK = 100;
    //手雷爆炸的伤害的线性衰减参数
    public const double GRENADE_DAMAGE_DECAY = 13.83;
    //手雷爆炸的伤害的最大值
    public const double GRENADE_MAX_DAMAGE = 108.07;
    //手雷爆炸的伤害的最大半径
    public const double GRENADE_MAX_RADIUS = 6;
    //玩家的碰撞箱的半径（圆形，单位为格数）
    public const double PLAYER_COLLISION_BOX = 0.5;
    //玩家初始背包大小
    public const int PLAYER_INITIAL_BACKPACK_SIZE = 150;

    public const int PLAYER_PICK_UP_DISTANCE = 1;

    public const double PLAYER_SPEED_PER_TICK = 0.2;

    public const int BANDAGE_HEAL = 10;

    public const int FIRST_AID_HEAL = 75;

    public const int PRIMARY_ARMOR_DEFENSE = 50;

    public const int PREMIUM_ARMOR_DEFENSE = 100;
}
