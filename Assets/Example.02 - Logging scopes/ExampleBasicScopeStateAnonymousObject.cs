using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ExampleBasicScopeStateAnonymousObject : MonoBehaviour
{
    public bool duplicateWithJson;
    private ILogger _logger = null!;
    private ILoggerFactory _loggerFactory = null!; // Store the factory

    void Awake()
    {
        // Create the factory and store it
        _loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Trace); // Log everything from Trace upwards

            // Configure Unity Debug Console output
            logging.AddZLoggerUnityDebug(options =>
            {
                options.IncludeScopes = true;
                options.UsePlainTextFormatter();
                options.PrettyStacktrace = true;
            });


            // Configure json
            if (duplicateWithJson)
                logging.AddZLoggerUnityDebug(options =>
                {
                    options.IncludeScopes = true;
                    options.UseJsonFormatter();
                });
        });

        // Create the logger instance using the stored factory
        // Use the actual class name for the category
        _logger = _loggerFactory.CreateLogger<ExampleBasicScopeStateAnonymousObject>();
        // Or use a specific category name string: _logger = _loggerFactory.CreateLogger("MyExampleCategory");

        using (_logger.BeginScope("InitializationScope")) // Changed scope name for clarity
        {
            _logger.ZLogInformation($"Logger setup complete.");
        }
    }

    void Start()
    {
        // Run example
        ExampleScopeWithState();
    }

    void ExampleScopeWithState()
    {
        var playerId = 99;
        var levelName = "QuantumCatNebula";

        // Structured scope with an anonymous object as state
        using (_logger.BeginScope(new { PlayerId = playerId, Level = levelName }))
        {
            _logger.ZLogInformation($"Player entered the level.");

            DoLevelLoading();

            _logger.ZLogInformation($"Level initialization complete.");
        }
    }

    void DoLevelLoading()
    {
        using (_logger.BeginScope("LevelLoadingPhase"))
        {
            _logger.ZLogDebug($"Loading assets...");
            _logger.ZLogDebug($"Initializing systems...");
        }
    }
}