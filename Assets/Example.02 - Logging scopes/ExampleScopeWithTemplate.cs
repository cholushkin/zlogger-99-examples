using UnityEngine;
using Microsoft.Extensions.Logging;
using ZLogger;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ExampleScopeWithTemplate : MonoBehaviour
{
    public LogLevel LogLevel = LogLevel.Debug;

    private ILogger _logger = null!;
    private ILoggerFactory _loggerFactory = null!;

    void Awake()
    {
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel);
            builder.AddZLoggerUnityDebug(options =>
            {
                options.IncludeScopes = true;
                options.UseJsonFormatter(); // Enables structured output
            });
        });

        _logger = _loggerFactory.CreateLogger<ExampleScopeWithTemplate>();
    }

    void Start()
    {
        int userId = 42;
        string action = "Jump";

        // ⚠️ Important Notes:
        // - This message template is interpreted as both:
        //     1. A human-readable message string
        //     2. A structured state container with named properties
        //
        // - The template `{UserId}` is not just replaced with 42 — 
        //   it becomes a named key `UserId = 42` in the structured scope.
        //
        // This is especially useful with JSON formatters, enabling log filtering and analysis.
        using (_logger.BeginScope("User {UserId} performed action {ActionType}", userId, action))
        {
            _logger.ZLogInformation($"Inside scope, hey!");
        }

        _logger.ZLogInformation($"Outside scope.");
    }

    void OnDestroy()
    {
        _loggerFactory?.Dispose();
    }
}