namespace GameServer.GameLogic;

public class Grenade : IGrenade
{
    //定点爆炸的坐标
    public Position position { get; set; }
    //爆炸的tick
    public int explodeTick { get; set; }
    //是否已经爆炸，初始为false
    public bool hasExploded { get; set; } = false;


    //构造函数：初始化手雷的爆炸位置、扔出的tick
    public Grenade(Position position, int throwTick)
    {
        this.position = position;
        explodeTick = throwTick + Constant.GRENADE_EXPLODE_TICK;
    }

    //判断手雷是否爆炸，如果tick>=ExplodeTick，爆炸，设HasExploded为True
    //  进行范围伤害计算并作用到player上，并return true;
    //否则return false
    public bool Explode(int tick, IPlayer[] players, Map map)
    {
        if (tick >= explodeTick)
        {
            hasExploded = true;
            foreach (IPlayer player in players)
            {
                player.TakeDamage(ComputeGrenadeDamage(position, player.PlayerPosition, map));
            }
            return true;
        }
        return false;
    }

    private static int ComputeGrenadeDamage(Position explodePosition, Position playerPosition, Map map)
    {
        //遍历以playerPosition为中心，Constant.PLAYER_COLLISION_BOX为半径的圆（以1度为微元）
        double damageSum = 0;
        for (int angle = 0; angle < 360; angle++)
        {
            double radians = angle * Math.PI / 180;
            double nx = playerPosition.x + Constant.PLAYER_COLLISION_BOX * Math.Cos(radians);
            double ny = playerPosition.y + Constant.PLAYER_COLLISION_BOX * Math.Sin(radians);
            Position pointOnCircle = new(nx, ny);
            double dis = Position.Distance(explodePosition, pointOnCircle);
            if (
                dis <= Constant.GRENADE_MAX_RADIUS
                && map.IsConnected(explodePosition, pointOnCircle)
                )
            {
                double damage = Constant.GRENADE_MAX_DAMAGE - Constant.GRENADE_DAMAGE_DECAY * dis;
                damageSum += damage;
            }
        }
        return (int)(damageSum / 360);
    }
}
