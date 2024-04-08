namespace GameServer.GameLogic;

public partial class Constant
{
    //玩家的碰撞箱的半径（圆形，单位为格数）
    public const double PLAYER_COLLISION_BOX = 0.5;
    //玩家初始背包大小
    public const int PLAYER_INITIAL_BACKPACK_SIZE = 150;

    public const int PLAYER_PICK_UP_DISTANCE = 1;

    public const double PLAYER_SPEED_PER_TICK = 0.2;
    public const int PLAYER_MAXIMUM_HEALTH = 100;
}
