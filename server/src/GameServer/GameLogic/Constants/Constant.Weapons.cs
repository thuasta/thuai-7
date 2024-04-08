namespace GameServer.GameLogic;
public partial class Constant
{
    public const string FIST = "FIST";
    public const string S686 = "S686";
    public const string VECTOR = "VECTOR";
    public const string M16 = "M16";
    public const string AWM = "AWM";

    public const float FIST_RANGE = 1;
    public const int FIST_DAMAGE = 20;
    public const int FIST_COOLDOWNTICKS = (int)(TICKS_PER_SECOND * 0.5);
    public const float S686_RANGE = 50;
    public const int S686_SINGLE_BULLET_DAMAGE = 25;
    //弹头数目
    public const int S686_BULLET_NUM = 9;
    //两颗弹头角度差
    public const int S686_DELTA_DEG = 3;
    public const int S686_COOLDOWNTICKS = (int)(TICKS_PER_SECOND * 0.2);
    public const float VECTOR_RANGE = 50;
    public const int VECTOR_DAMAGE = 34;
    public const int VECTOR_COOLDOWNTICKS = (int)(TICKS_PER_SECOND * 0.054);
    public const float M16_RANGE = 100;
    public const int M16_DAMAGE = 43;
    public const int M16_COOLDOWNTICKS = (int)(TICKS_PER_SECOND * 0.12);
    public const float AWM_RANGE = 120;
    public const int AWM_DAMAGE = 132;
    public const int AWM_COOLDOWNTICKS = (int)(TICKS_PER_SECOND * 1.85);
}
