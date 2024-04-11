using System.Text.Json;
using GameServer.Connection;
using GameServer.GameController;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;

namespace GameServer;

class Program
{
    const string SerilogTemplate
        = "[{@t:HH:mm:ss} {@l:u3}] {#if Component is not null}{Component,-13} {#end}{@m}\n{@x}";

    static void Main(string[] args)
    {
        // Load config
        // Read the config file and deserialize it into a Config object.
        // string configJsonStr = File.ReadAllText("config.json");
        string configJsonStr = "{\"log_level\": \"INFORMATION\"}";

        Config config = JsonSerializer.Deserialize<Config>(configJsonStr) ?? new();

        SetLogLevel(config.LogLevel);

        ILogger _logger = Log.ForContext("Component", "GameServer");

        Version version = typeof(Program).Assembly.GetName().Version ?? new Version(0, 0, 0, 0);
        _logger.Information($"THUAI7 GameServer v{version.Major}.{version.Minor}.{version.Build}");
        _logger.Information("Copyright (c) 2024 THUASTA");

        IGameRunner gameRunner = new GameRunner(config);

        AgentServer agentServer = new()
        {
            Port = config.ServerPort
        };

        SubscribeEvents();

        try
        {
            agentServer.Start();

            // Wait for players to connect
            Task.Delay((int)(config.WaitingTime * 1000)).Wait();

            while (gameRunner.Game.PlayerCount < config.ExpectedPlayerNum)
            {
                _logger.Information($"Waiting for {config.ExpectedPlayerNum - gameRunner.Game.PlayerCount} more players to join...");
                Task.Delay(1000).Wait();
            }

            gameRunner.Start();

            while (true)
            {
                // TODO: Read commands from console
                string? input = Console.ReadLine();
                if (input == "stop")
                {
                    gameRunner.Stop();
                    Environment.Exit(0);
                }
                else
                {
                    _logger.Error($"Unknown command: {input}. Please check that the command exists and that you have permission to use it.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Fatal($"GameServer crashed with exception: {ex}");
        }

        void SubscribeEvents()
        {
            gameRunner.Game.AfterGameTickEvent += agentServer.HandleAfterGameTickEvent;
            agentServer.AfterMessageReceiveEvent += gameRunner.HandleAfterMessageReceiveEvent;
        }
    }

    static void SetLogLevel(string logLevel)
    {
        Log.Logger = logLevel switch
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
    }
}
