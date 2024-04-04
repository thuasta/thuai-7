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

        // Load config
        // Read the config file and deserialize it into a Config object.
        // string configJsonStr = File.ReadAllText("config.json");
        string configJsonStr = "{\"log_level\": \"VERBOSE\"}";

        Config config = JsonSerializer.Deserialize<Config>(configJsonStr) ?? new();

        Log.Logger = config.LogLevel switch
        {
            "VERBOSE" => new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(new ExpressionTemplate(SerilogTemplate, theme: TemplateTheme.Literate))
                .CreateLogger(),

            "DEBUG" => new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(new ExpressionTemplate(SerilogTemplate, theme: TemplateTheme.Literate))
                .CreateLogger(),

            "INFORMATION" => new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(new ExpressionTemplate(SerilogTemplate, theme: TemplateTheme.Literate))
                .CreateLogger(),

            "WARNING" => new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Console(new ExpressionTemplate(SerilogTemplate, theme: TemplateTheme.Literate))
                .CreateLogger(),

            "ERROR" => new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.Console(new ExpressionTemplate(SerilogTemplate, theme: TemplateTheme.Literate))
                .CreateLogger(),

            "FATAL" => new LoggerConfiguration()
                .MinimumLevel.Fatal()
                .WriteTo.Console(new ExpressionTemplate(SerilogTemplate, theme: TemplateTheme.Literate))
                .CreateLogger(),

            _ => new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(new ExpressionTemplate(SerilogTemplate, theme: TemplateTheme.Literate))
                .CreateLogger()
        };

        ILogger _logger = Log.ForContext("Component", "GameServer");

        Version version = typeof(Program).Assembly.GetName().Version ?? new Version(0, 0, 0, 0);
        _logger.Information($"THUAI7 GameServer v{version.Major}.{version.Minor}.{version.Build}");
        _logger.Information("Copyright (c) 2024 THUASTA");

        GameController.IGameRunner gameRunner = new GameController.GameRunner(config, _logger);

        AgentServer agentServer = new()
        {
            Port = config.ServerPort
        };

        try
        {
            // TODO: Activate and run game server
            agentServer.Start();

            // Wait WaitingTime seconds for players to connect and start the gameRunner
            System.Threading.Thread.Sleep((int)(config.WaitingTime * 1000));
            gameRunner.Start();
            while (true)
            {
                // TODO: Read commands from console
            }
        }
        catch (Exception ex)
        {
            _logger.Fatal($"GameServer crashed with exception: {ex.Message}");
        }
    }
}
