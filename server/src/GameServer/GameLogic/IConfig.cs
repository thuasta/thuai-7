namespace GameServer.GameLogic;

public interface IConfig
{
  public int TicksPerSecond { get; }

  public int PlayerSpawnMaxY { get; }
}