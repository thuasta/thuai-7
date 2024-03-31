namespace GameServer.GameLogic;

public partial class Game
{
    //创建一个Grenade对象构成的列表
    private readonly List<IPlayer> _players = new();

    public void AddPlayer(IPlayer player)
    {
        _players.Add(player);
    }

    public void RemovePlayer(IPlayer player)
    {
        _players.Remove(player);
    }

    public List<IPlayer> GetPlayers()
    {
        return _players;
    }

}