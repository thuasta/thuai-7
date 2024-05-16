using GameServer.Geometry;

namespace GameServer.GameLogic;

public class Grenade
{
    //定点爆炸的坐标
    public Position Position { get; set; }
    //爆炸的tick
    public int ExplodeTick { get; set; }
    public int ThrowTick { get; init; }
    //是否已经爆炸，初始为false
    public bool HasExploded { get; set; } = false;

    public Position EvaluatedPosition { get; private set; }

    private readonly Random _random = new();

    //构造函数：初始化手雷的爆炸位置、扔出的tick
    public Grenade(Position position, int throwTick)
    {
        Position = position;
        ThrowTick = throwTick;
        ExplodeTick = throwTick + Constant.GRENADE_EXPLODE_TICK;

        double evalX = 2 * (_random.NextDouble() - 0.5) * Constant.GRENADE_MAX_RADIUS;
        double evalY = 2 * (_random.NextDouble() - 0.5) * Constant.GRENADE_MAX_RADIUS;

        while (new Position(evalX, evalY).Length() > Constant.GRENADE_MAX_RADIUS)
        {
            evalX = 2 * (_random.NextDouble() - 0.5) * Constant.GRENADE_MAX_RADIUS;
            evalY = 2 * (_random.NextDouble() - 0.5) * Constant.GRENADE_MAX_RADIUS;
        }

        EvaluatedPosition = new(
            Position.x + evalX,
            Position.y + evalY
        );
    }

    //判断手雷是否爆炸，如果tick>=ExplodeTick，爆炸，设HasExploded为True
    //  进行范围伤害计算并作用到player上，并return true;
    //否则return false
    public bool Explode(int tick, Player[] players, Map map, List<Recorder.IRecord> _events)
    {
        List<Recorder.GrenadeExplodeRecord.Victim> _victims = new();
        if (tick >= ExplodeTick && !HasExploded)
        {
            HasExploded = true;
            foreach (Player player in players)
            {
                int tempHurt = ComputeGrenadeDamage(Position, player.PlayerPosition, map);
                player.TakeDamage(tempHurt);
                _victims.Add(new Recorder.GrenadeExplodeRecord.Victim()
                {
                    token = player.Token,
                    hurt = tempHurt
                });
            }

            Recorder.GrenadeExplodeRecord record = new()
            {
                ExplodePosition = new()
                {
                    x = Position.x,
                    y = Position.y
                },
                Victims = new(_victims)
            };
            _events.Add(record);
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
