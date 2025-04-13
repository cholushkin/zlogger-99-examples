using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ExampleLoggingScopes : MonoBehaviour
{
    public bool duplicateWithJson;
    public LogLevel minimumLevel;
    
    private ILogger _logger;

    void Awake()
    {
        using var loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.SetMinimumLevel(minimumLevel);
            logging.AddZLoggerUnityDebug(
                options =>
                {
                    options.UsePlainTextFormatter();
                    options.IncludeScopes = true; // Enable scope inclusion in output
                }
            );

            if (duplicateWithJson)
                logging.AddZLoggerUnityDebug(options =>
                {
                    options.UseJsonFormatter();
                    options.IncludeScopes = true; // Enable scope inclusion in output
                });
        });

        _logger = loggerFactory.CreateLogger<ExampleLoggingScopes>(); // logger with a category of specified class "ExampleLoggingScopes" 
        _logger.ZLog(LogLevel.Information,$"Logger is created");
    }

    void Start()
    {
        using (_logger.BeginScope("Request: 123"))
        {
            _logger.ZLogInformation($"Processing order.");
            using (_logger.BeginScope("Database"))
            {
                _logger.ZLogDebug($"Query: SELECT * FROM Orders");
            }
            _logger.ZLogInformation($"Order processed.");
        }
    
        // // Example 2: Scopes with IDs and Objects
        // int playerId = 12345;
        // string playerName = "Hero";
        // var playerStats = new { Health = 100, Mana = 50, Level = 10 };
        //
        // using (_logger.BeginScope("Player Action", new { PlayerId = playerId, PlayerName = playerName }))
        // {
        //     _logger.ZLogInformation($"Player logged in. Player Stats: {Stats}", playerStats);
        //
        //     using (_logger.BeginScope("Combat", new { Target = "Enemy", Damage = 20 }))
        //     {
        //         _logger.ZLogWarning($"Player attacked Enemy for {Damage} damage.", 20);
        //         _logger.ZLogInformation($"Enemy health remaining: {Health}", 30);
        //     }
        //
        //     _logger.ZLogInformation("Player action complete");
        // }
    
        // // Example 3: Exception Handling with Scopes
        // try
        // {
        //     using (_logger.BeginScope("File Operation"))
        //     {
        //         _logger.ZLogInformation($"Attempting to open file: 'data.txt'");
        //         throw new System.IO.FileNotFoundException("Could not find file 'data.txt'.");
        //     }
        // }
        // catch (Exception ex)
        // {
        //     _logger.ZLogError(ex, $"An error occurred during file operation.");
        // }
    }
    
    void Update()
    {
        // using (_logger.BeginScope("Update()"))
        // {
        //     // Log something every few frames, to show how scopes can be used in Update
        //     if (Time.frameCount % 30 == 0)
        //     {
        //         _logger.ZLogDebug($"Updating game state...");
        //         _logger.ZLogTrace($"Current position: {transform.position}");
        //     }
        // }
    }
    
    void OnDestroy()
    {
        using (_logger.BeginScope("OnDestroy()"))
        {
            _logger.ZLogInformation($"OnDestroy called.  Cleaning up...");
        }
    }
}