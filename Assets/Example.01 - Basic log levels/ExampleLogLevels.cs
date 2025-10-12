using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ExampleLogLevels : MonoBehaviour
{
    public LogLevel minimumLevel;
    public bool duplicateWithJson;

    private ILogger _logger = null!;
    private ILoggerFactory _loggerFactory = null!; // Store the factory instance


    void Awake()
    {
        // Create and store the factory
        _loggerFactory = LoggerFactory.Create(logging =>
        {
            // Set the minimum level for all providers added HERE
            logging.SetMinimumLevel(minimumLevel);

            // Add provider for plain text output to Unity Console
            logging.AddZLoggerUnityDebug(options =>
                {
                    options.UsePlainTextFormatter(); // Use plain text
                    options.PrettyStacktrace = true; // Format stack traces nicely
                    // Optional: Configure scope/prefix/suffix for plain text if needed
                    // options.UsePlainTextFormatter(cfg => { /* ... custom config ... */ });
                }
            );

            // Conditionally add a second provider for JSON output to Unity Console
            if (duplicateWithJson)
            {
                logging.AddZLoggerUnityDebug(options => options.UseJsonFormatter());
            }
        });

        // Create the logger instance from the stored factory
        _logger = _loggerFactory.CreateLogger("ZLoggerExample"); // Category name

        // Log confirmation (will use the configured formatters)
        _logger.ZLogInformation($"Logger created in Awake. Minimum Level: {minimumLevel}");
    }

    void Start()
    {
        _logger.ZLogInformation($"with context {name}", this);

        /*
            ✅ What it is:
            Comes from Microsoft.Extensions.Logging.
            Part of the standard logging API.
            Accepts structured messages or raw strings.
            Internally uses string formatting like string.Format(...).
            ❌ Downsides:
            Allocates memory when interpolated strings or string.Format(...) are used.
            Not GC-friendly in high-frequency logging.
        */
        _logger.Log(LogLevel.Information, $"LogInformation");
        _logger.LogInformation($"LogInformation");

        /*
            ✅ What it is:
            Extension method provided by ZLogger.
            Optimized for performance.
            Uses ZString and UTF8 formatting for zero-alloc logging (when used correctly).
            Designed for high-throughput systems (games, servers, etc.).
            🚀 Benefits:
            No GC allocations when using parameters properly.
            Can log structured data without creating intermediate strings.
         */
        
        // https://devblogs.microsoft.com/dotnet/string-interpolation-in-c-10-and-net-6/

        _logger.ZLogInformation($"LogInformation");
        _logger.ZLogTrace($"This is a TRACE log.");
        _logger.ZLogDebug($"This is a DEBUG log.");
        _logger.ZLogWarning($"This is a WARNING log.");
        _logger.ZLogError($"This is an ERROR log.");
        _logger.ZLogCritical($"This is a CRITICAL log.");
    }
}