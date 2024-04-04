using Serilog;

namespace GameServer.Security;

public class WatchDog
{
    public static void Feed(WatchDog dog)
    {
        dog._lastFeedTime = DateTime.Now;
        dog._logger.Verbose($"WatchDog fed at {dog._lastFeedTime}.");
    }

    private readonly int _maxFeedInterval;
    private DateTime _lastFeedTime = DateTime.Now;

    private Task? _taskForWatching = null;

    private readonly ILogger _logger = Log.Logger.ForContext("Component", "WatchDog");

    public WatchDog(int maxFeedInterval)
    {
        _maxFeedInterval = maxFeedInterval;
    }

    public void Watch(string taskName, Task task, Action actionForRestartingTask)
    {
        if (_taskForWatching is not null)
        {
            _logger.Error("WatchDog is already watching a task.");
            return;
        }

        _lastFeedTime = DateTime.Now;

        _taskForWatching = Task.Run(() =>
        {
            while (task.Status == TaskStatus.Running)
            {
                Task.Delay(_maxFeedInterval).Wait();

                if (DateTime.Now - _lastFeedTime > TimeSpan.FromMilliseconds(_maxFeedInterval))
                {
                    _logger.Error(
                        $"\"{taskName}\" (with Task Id {task.Id}) doesn't feed dog for more than {_maxFeedInterval} ms."
                    );
                    _logger.Error("Restarting \"{taskName}\".");
                    task.Dispose();
                    task = Task.Run(actionForRestartingTask);
                }
            }
        });
    }

    public void Stop()
    {
        _taskForWatching?.Dispose();
        _taskForWatching = null;
    }
}
