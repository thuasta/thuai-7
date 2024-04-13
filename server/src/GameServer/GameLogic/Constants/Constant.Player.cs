namespace GameServer.GameLogic;

public partial class Constant
{
    // Collision box of player (only for collision with bullet, not for collision with wall)
    public const double PLAYER_COLLISION_BOX = 0.5;

    // Initial backpack size of player
    public const int PLAYER_INITIAL_BACKPACK_SIZE = 50;

    public const int PLAYER_PICK_UP_DISTANCE = 1;

    public const double PLAYER_SPEED_PER_TICK = 0.2;
    public const int PLAYER_MAXIMUM_HEALTH = 100;
}
