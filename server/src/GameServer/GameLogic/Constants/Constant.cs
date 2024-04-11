namespace GameServer.GameLogic;

public partial class Constant
{
    public const int BANDAGE_HEAL = 10;
    public const int FIRST_AID_HEAL = 75;
    //手榴弹爆炸延时的tick数
    public const int GRENADE_EXPLODE_TICK = 100;
    //手雷爆炸的伤害的线性衰减参数
    public const double GRENADE_DAMAGE_DECAY = 13.83;
    //手雷爆炸的伤害的最大值
    public const double GRENADE_MAX_DAMAGE = 108.07;
    //手雷爆炸的伤害的最大半径
    public const double GRENADE_MAX_RADIUS = 6;

    public const int NO_ARMOR_DEFENSE = 0;
    public const int PRIMARY_ARMOR_DEFENSE = 50;

    public const int PREMIUM_ARMOR_DEFENSE = 100;

}
