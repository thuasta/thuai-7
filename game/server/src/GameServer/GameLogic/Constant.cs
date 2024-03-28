namespace GameServer.GameLogic;
public class Constant
{
    //手榴弹爆炸延时的tick数
    //TODO 待修改
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

    public const int MEDICINE_HEAL = 30;
}
