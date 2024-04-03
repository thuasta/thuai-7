using GameServer.GameLogic;

namespace GameServer.GameController;

public class GameRunner : IGameRunner
{
    public Game Game { get; }
    private readonly Task _tickTask;
    public GameRunner(Config config)
    {
        Game = new Game(config);

        _tickTask = new Task(() =>
        {
            while (true)
            {
                Game.Tick();
            }
        });
    }

    public void Start()
    {

        _tickTask.Start();
        // TODO: Implement
    }

    public void Stop()
    {
        _tickTask.Wait();
        Game.Stop();
    }

    public void Reset()
    {
        // TODO: Implement
        throw new NotImplementedException();
    }
}
