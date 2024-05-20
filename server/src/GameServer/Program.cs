using System.Text.Json;
using GameServer.Connection;
using GameServer.GameController;
using GameServer.GameLogic;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;

namespace GameServer;

class Program
{
    const int TIMEOUT_EXIT_CODE = 2;

    const string SerilogTemplate
        = "[{@t:HH:mm:ss.fff} {@l:u3}] {#if Component is not null}{Component,-13} {#end}{@m}\n{@x}";
    const string SerilogFileOutputTemplate
        = "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Component,-13:default(No Component)} {Message:lj}{NewLine}{Exception}";

    static void Main(string[] args)
    {
        if (File.Exists("config.json") == false)
        {
            File.WriteAllText(
                "config.json",
                JsonSerializer.Serialize(
                    new Config(),
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }
                )
            );
        }
        string configJsonStr = File.ReadAllText("config.json");
        Config config = JsonSerializer.Deserialize<Config>(configJsonStr) ?? new();

        SetLog(config.LogTarget, config.LogLevel);

        ILogger _logger = Log.ForContext("Component", "GameServer");

        Version version = typeof(Program).Assembly.GetName().Version ?? new Version(0, 0, 0, 0);

        string? allTokensStr = Environment.GetEnvironmentVariable(config.TokenListEnv);
        List<string> allTokens = allTokensStr?.Split(';').ToList() ?? new();

        bool useWhiteList = (allTokens.Count > 0);

        Task.Run(() =>
            {
                Task.Delay(config.MaxRunningSeconds * 1000).Wait();
                _logger.Error(
                    $"GameServer has been running for {config.MaxRunningSeconds} seconds. Stopping..."
                );
                Environment.Exit(TIMEOUT_EXIT_CODE);
            }
        );

        try
        {
            Welcome();

            _logger.Information($"WhiteListMode: {(useWhiteList ? "ON" : "OFF")}");
            if (useWhiteList == true)
            {
                _logger.Debug("WhiteList:");
                foreach (string token in allTokens)
                {
                    _logger.Debug(token.Length > 16 ? string.Concat(token.AsSpan(0, 16), "...") : token);
                }
            }

            GameRunner gameRunner = new(config)
            {
                WhiteListMode = useWhiteList,
                WhiteList = new(allTokens)
            };

            AgentServer agentServer = new()
            {
                Port = config.ServerPort,
                WhiteListMode = useWhiteList,
                WhiteList = new(allTokens)
            };

            SubscribeEvents();
            agentServer.Start();

            bool allConnected = false;
            bool forceStart = false;

            Task.Run(() =>
            {
                Task.Delay(config.ConnectionLimitTime * 1000).Wait();
                if (allConnected == false)
                {
                    if (useWhiteList == true)
                    {
                        _logger.Warning(
                            $"Connected clients are not enough. Force starting the game with {allTokens.Count} players..."
                        );
                        gameRunner.AllocatePlayer(allTokens);
                        forceStart = true;
                    }
                    else
                    {
                        _logger.Error(
                            $"Connected clients are not enough. Stopping..."
                        );
                        Environment.Exit(TIMEOUT_EXIT_CODE);
                    }
                }
            });

            // Wait for players to connect or until force start
            Task.Delay(config.QueueTime * 1000).Wait();

            while (gameRunner.Game.PlayerCount < config.PlayerCount && forceStart == false)
            {
                _logger.Information(
                    $"Waiting for {config.PlayerCount - gameRunner.Game.PlayerCount} more players to join..."
                );
                Task.Delay(1000).Wait();
            }

            allConnected = true;

            gameRunner.Start();

            HandleCommand();

            while (true)
            {
                Task.Delay(100).Wait();
                if (gameRunner.Game.Stage == Game.GameStage.Finished)
                {
                    gameRunner.Stop(forceStop: false);
                    break;
                }
            }

            #region Local Functions
            void SubscribeEvents()
            {
                gameRunner.Game.AfterGameTickEvent += agentServer.HandleAfterGameTickEvent;
                gameRunner.AfterPlayerConnectEvent += agentServer.HandleAfterPlayerConnectEvent;
                agentServer.AfterMessageReceiveEvent += gameRunner.HandleAfterMessageReceiveEvent;
            }

            void HandleCommand()
            {
                ILogger loggerForConsole = Log.ForContext("Component", "Console");
                Task taskForHandlingCommand = Task.Run(() =>
                {
                    while (true)
                    {
                        Task.Delay(100).Wait();

                        string? input = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(input) == true)
                        {
                            continue;
                        }

                        switch (input)
                        {
                            case "stop":
                                gameRunner.Stop(forceStop: true);
                                Environment.Exit(0);
                                break;

                            default:
                                loggerForConsole.Error(
                                    $"Unknown command: \"{input}\"."
                                );
                                break;
                        }
                    }
                });
            }

            void Welcome()
            {
                _logger.Information(
                    @"
 .----------------.  .----------------.  .----------------.  .----------------.  .----------------.
| .--------------. || .--------------. || .--------------. || .--------------. || .--------------. |
| |  _________   | || |  ____  ____  | || | _____  _____ | || |   ______     | || |    ______    | |
| | |  _   _  |  | || | |_   ||   _| | || ||_   _||_   _|| || |  |_   _ \    | || |  .' ___  |   | |
| | |_/ | | \_|  | || |   | |__| |   | || |  | |    | |  | || |    | |_) |   | || | / .'   \_|   | |
| |     | |      | || |   |  __  |   | || |  | '    ' |  | || |    |  __'.   | || | | |    ____  | |
| |    _| |_     | || |  _| |  | |_  | || |   \ `--' /   | || |   _| |__) |  | || | \ `.___]  _| | |
| |   |_____|    | || | |____||____| | || |    `.__.'    | || |  |_______/   | || |  `._____.'   | |
| |              | || |              | || |              | || |              | || |              | |
| '--------------' || '--------------' || '--------------' || '--------------' || '--------------' |
 '----------------'  '----------------'  '----------------'  '----------------'  '----------------' "
                );
                _logger.Information(
                    "\n" + @"
        #                #            #     #  #       #         #
       ##               ##           ##    ## ##      ##   #######
       ##  #            ##   #       ## #  ##  ##     ##       ##
   ##########      ############      ##### ##  #      ##      ##
       ##           #   ##  #        ##    ######   ######   ##
       ##    #       ## ##  ##       ##  ####         ##    ##    #
  #############       #### ##      # ## #  ##  #      ##  #########
      ###             # ####  #    ####### ##  ##     ##   #  #  ##
     #####        ##############   ##  ##  ## ##      ##   ## ## ##
     #### #            ####        ##  ##   ####      ## # ## ## ##
    ## ## ##          ######       ##  ##   ###       ### ## ##  ##
    ## ##  ##        ## ## ##      ##  ##   ##      ####  #  ##  ##
   ##  ##  ###      ##  ##  ###    ##  ##  ####  #   #   #  ##   ##
  ##   ##   ####   ##   ##   ####  ###### ##  ## #      #  ##   ##
 #     ##     #   #     ##    ##   #   # ##    ###        ##  ####
       #                #               #       ##      ##      #   "
                );
                _logger.Information($"THUAI7 GameServer v{version.Major}.{version.Minor}.{version.Build}");
                _logger.Information("Copyright (c) 2024");
                _logger.Information(
                    "Student Association of Science and Technology, Department of Automation, Tsinghua University"
                );
                _logger.Information(
                    "--------------------------------------------------------------------------------------------\n"
                );
            }
            #endregion
        }
        catch (Exception ex)
        {
            _logger.Fatal($"GameServer crashed with exception: {ex}");
            _logger.Fatal("Press Ctrl+C to exit.");
            Task.Delay(-1).Wait();
        }
    }

    static void SetLog(string logTarget, string logLevel)
    {
        try
        {
            ExpressionTemplate template = new(SerilogTemplate, theme: TemplateTheme.Literate);

            if (logTarget == "CONSOLE")
            {
                Log.Logger = logLevel switch
                {
                    "VERBOSE" => new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .WriteTo.Console(template)
                        .CreateLogger(),

                    "DEBUG" => new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console(template)
                        .CreateLogger(),

                    "INFORMATION" => new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .WriteTo.Console(template)
                        .CreateLogger(),

                    "WARNING" => new LoggerConfiguration()
                        .MinimumLevel.Warning()
                        .WriteTo.Console(template)
                        .CreateLogger(),

                    "ERROR" => new LoggerConfiguration()
                        .MinimumLevel.Error()
                        .WriteTo.Console(template)
                        .CreateLogger(),

                    "FATAL" => new LoggerConfiguration()
                        .MinimumLevel.Fatal()
                        .WriteTo.Console(template)
                        .CreateLogger(),

                    _ => throw new ArgumentException($"Invalid log level: {logLevel}")
                };
            }
            else
            {
                if (File.Exists(logTarget) == true)
                {
                    throw new InvalidOperationException($"Writing to an existing file is forbidden.");
                }

                File.Create(logTarget).Close();

                Log.Logger = logLevel switch
                {
                    "VERBOSE" => new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .WriteTo.File(logTarget, outputTemplate: SerilogFileOutputTemplate)
                        .CreateLogger(),

                    "DEBUG" => new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.File(logTarget, outputTemplate: SerilogFileOutputTemplate)
                        .CreateLogger(),

                    "INFORMATION" => new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .WriteTo.File(logTarget, outputTemplate: SerilogFileOutputTemplate)
                        .CreateLogger(),

                    "WARNING" => new LoggerConfiguration()
                        .MinimumLevel.Warning()
                        .WriteTo.File(logTarget, outputTemplate: SerilogFileOutputTemplate)
                        .CreateLogger(),

                    "ERROR" => new LoggerConfiguration()
                        .MinimumLevel.Error()
                        .WriteTo.File(logTarget, outputTemplate: SerilogFileOutputTemplate)
                        .CreateLogger(),

                    "FATAL" => new LoggerConfiguration()
                        .MinimumLevel.Fatal()
                        .WriteTo.File(logTarget, outputTemplate: SerilogFileOutputTemplate)
                        .CreateLogger(),

                    _ => throw new ArgumentException($"Invalid log level: {logLevel}")
                };
            }

            Log.ForContext("Component", "Logger").Information(
                $"Log target set to {logTarget} with level {logLevel}."
            );
            Task.Delay(1000).Wait();
        }
        catch (Exception ex)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(new ExpressionTemplate(SerilogTemplate, theme: TemplateTheme.Literate))
                .CreateLogger();
            Log.ForContext("Component", "Logger").Error(
                $"Failed to set log target to {logTarget} with level {logLevel}: {ex.Message}"
            );
            Log.ForContext("Component", "Logger").Debug($"{ex}");
            Log.ForContext("Component", "Logger").Error(
                $"Using default log target: CONSOLE with level INFORMATION."
            );
            Task.Delay(1000).Wait();
        }
    }
}
