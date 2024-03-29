using Serilog;

namespace GameServer;

class Program
{
    static void Main()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();

        ILogger _logger = Log.ForContext("Component", "GameServer");

        Version version = typeof(Program).Assembly.GetName().Version ?? new Version(0, 0, 0, 0);
        _logger.Information($"THUAI7 GameServer v{version.Major}.{version.Minor}.{version.Build}");
        _logger.Information("Copyright (c) 2024 THUASTA");

        try
        {
            // TODO: Activate and run game server

            throw new NotImplementedException("GameServer is not implemented yet.");
        }
        catch (Exception ex)
        {
            _logger.Fatal($"Game server crashed with exception: {ex.Message}");
        }
    }
}
