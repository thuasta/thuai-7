using System.Text.Json;
using Serilog;

namespace GameServer;

class Program
{
    static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();

        ILogger _logger = Log.ForContext("Component", "GameServer");

        Version version = typeof(Program).Assembly.GetName().Version ?? new Version(0, 0, 0, 0);
        _logger.Information($"THUAI7 GameServer v{version.Major}.{version.Minor}.{version.Build}");
        _logger.Information("Copyright (c) 2024 THUASTA");


        // Load config
        // Read the config file and deserialize it into a Config object.
        // string configJsonStr = File.ReadAllText("config.json");
        string configJsonStr = "{}";

        Config config = JsonSerializer.Deserialize<Config>(configJsonStr)!;

        GameController.IGameRunner gameRunner = new GameController.GameRunner(config, _logger);

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
