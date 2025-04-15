using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ExampleScopeWithCustomObject : MonoBehaviour
{
    [Tooltip("Set to 'None' to test that logs are disabled, but the scope and internal logic still execute.")]
    public LogLevel LogLevel;

    private ILogger _logger = null!;
    private ILoggerFactory _loggerFactory = null!;

    // A simple data object representing the scope's state
    private class Player
    {
        public Player(string name, int id, int level)
        {
            Name = name;
            Id = id;
            Level = level;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }

        // Optional: override ToString for plain text fallback
        public override string ToString() => $"Player(Id={Id}, Name={Name}, Level={Level})";
    }

    void Awake()
    {
        // Create and configure the logging factory
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel);

            builder.AddZLoggerUnityDebug(options =>
            {
                options.IncludeScopes = true;
                options.UseJsonFormatter(); // Structured format for scope visibility
            });
        });

        _logger = _loggerFactory.CreateLogger<ExampleScopeWithCustomObject>();
    }

    void Start()
    {
        Example();
    }
    
    private void Example()
    {
        var player = new Player ( "HeroCat", 42, 80 );

        // The Player object becomes the structured state for the scope
        using (_logger.BeginScope(player))
        {
            // Logic inside the scope
            _logger.ZLogInformation($"Player performed an action.");
            NestedCall();
            GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = new Vector3(1, 0, 0);
        }

        _logger.ZLogInformation($"Outside of player scope.");
    }

    private void NestedCall()
    {
        _logger.ZLogInformation($"Player nested call still has scope state in the log message. ");
    }

    void OnDestroy()
    {
        _loggerFactory?.Dispose();
    }
}