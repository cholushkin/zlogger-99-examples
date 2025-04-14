using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ExampleScope : MonoBehaviour
{
    private ILogger _logger;
    private ILoggerFactory _loggerFactory; // Store the factory

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
            logging.AddZLoggerUnityDebug(options =>
            {
                options.IncludeScopes = true;
                options.UseJsonFormatter(); 
            });
        });

        // Create the logger instance using the stored factory
        // Use the actual class name for the category
        _logger = _loggerFactory.CreateLogger<ExampleScope>();
        // Or use a specific category name string: _logger = _loggerFactory.CreateLogger("MyExampleCategory");

        using (_logger.BeginScope("InitializationScope")) // Changed scope name for clarity
        {
            _logger.ZLogInformation($"Logger setup complete."); 
        }
    }

    void Start()
    {
        // Run example
        ExampleScopeBasic();
        ExampleScopeWithState();
    }

    void ExampleScopeBasic()
    {
        // Example logging with scopes in Start
        using (_logger.BeginScope("order-processing")) // Use structured logging for scope values
        {
            _logger.ZLogInformation($"Processing order started.");
            using (_logger.BeginScope("query")) // More specific scope name
            {
                _logger.ZLogDebug($"Querying orders: SELECT * FROM Orders WHERE Id = {0}", 32); // Example structured log
            }

            _logger.ZLogInformation($"Order processed successfully.");
        }
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