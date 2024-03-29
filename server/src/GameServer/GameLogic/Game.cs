namespace GameServer.GameLogic;

public partial class Game : IGame
{
    //创建一个Grenade对象构成的列表
    private readonly List<IGrenade> _grenades = new();
    public void Tick()
    {
        // TODO:Implement
        throw new NotImplementedException();
    }
}
