using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ExampleScopeWithInterpolatedString : MonoBehaviour
{
    [Tooltip("Set to 'None' to test that logs are disabled, but the scope and internal logic still execute.")]
    public LogLevel LogLevel;

    private ILogger _logger = null!;
    private ILoggerFactory _loggerFactory = null!;

    void Awake()
    {
        // Create and configure the logging factory
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            // Set the global log level (can be 'None' to disable all logs)
            builder.SetMinimumLevel(LogLevel);

            // Configure Unity Debug output using ZLogger's JSON formatter
            builder.AddZLoggerUnityDebug(options =>
            {
                options.IncludeScopes = true;
                options.UseJsonFormatter(); // Structured format for scope visibility
            });
        });

        // Create logger with the current MonoBehaviour's type
        _logger = _loggerFactory.CreateLogger<ExampleScopeWithInterpolatedString>();
    }

    void Start()
    {
        var userId = 42;
        var sessionId = "abc-123";

        // Interpolated string scope — useful for dynamic context visibility
        using (_logger.BeginScope($"UserSession: userId={userId}, sessionId={sessionId}"))
        {
            // Code inside this scope is always executed,
            // even if the log level is too low to print any messages

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = Vector3.zero;
            cube.name = "SpawnedCube";

            _logger.ZLogInformation($"User session started.");
        }

        // Log outside the scope
        _logger.ZLogInformation($"Outside of user session scope.");
    }

    void OnDestroy()
    {
        // Always clean up your logger factory
        _loggerFactory?.Dispose();
    }
}