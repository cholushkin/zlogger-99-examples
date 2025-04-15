using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using System.Collections.Generic;

public class ExampleScopeWithDynamicDictionary : MonoBehaviour
{
    [Header("Logging Controls")]
    [Tooltip("Set to 'None' to disable logs.")]
    public LogLevel LogLevel = LogLevel.Information;

    [Header("Simulated User Input")]
    public int UserId = 101;
    public string SessionId = "session-xyz";
    public string Action = "explore";
    public bool IsAdmin = false;
    public string Region = "EU";

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
                options.UseJsonFormatter(); // For structured scope fields
            });
        });

        _logger = _loggerFactory.CreateLogger<ExampleScopeWithDynamicDictionary>();
    }

    void Start()
    {
        var scopeState = new Dictionary<string, object>
        {
            ["userId"] = UserId,
            ["sessionId"] = SessionId,
            ["action"] = Action
        };

        // Dynamically add optional properties
        if (IsAdmin)
        {
            scopeState["role"] = "admin";
        }

        if (!string.IsNullOrWhiteSpace(Region))
        {
            scopeState["region"] = Region;
        }

        using (_logger.BeginScope(scopeState))
        {
            _logger.ZLogInformation($"User performed a contextual action.");
        }

        _logger.ZLogInformation($"This is outside of dynamic scope.");
    }

    void OnDestroy()
    {
        _loggerFactory?.Dispose();
    }
}