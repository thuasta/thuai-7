using System.Text.Json;
using GameServer.Connection;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;

namespace GameServer;

class Program
{
    static void Main(string[] args)
    {
        const string SerilogTemplate
            = "[{@t:HH:mm:ss} {@l:u3}] {#if Component is not null}{Component,-13} {#end}{@m}\n{@x}";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(new ExpressionTemplate(SerilogTemplate, theme: TemplateTheme.Literate))
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
            _logger.Fatal($"GameServer crashed with exception: {ex.Message}");
        }
    }
}
