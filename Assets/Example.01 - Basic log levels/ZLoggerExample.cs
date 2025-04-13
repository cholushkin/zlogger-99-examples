using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ZLoggerExample : MonoBehaviour
{
    public LogLevel minimumLevel;
    public bool duplicateWithJson;
    
    private ILogger _logger;

    void Awake()
    {
        using var loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.SetMinimumLevel(minimumLevel);
            logging.AddZLoggerUnityDebug(options =>
            {
                options.UsePlainTextFormatter();
            });
            
            if(duplicateWithJson)
                logging.AddZLoggerUnityDebug(options =>
                {
                    options.UseJsonFormatter();
                });

        });

        _logger = loggerFactory.CreateLogger("ZLoggerExample");
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

        _logger.ZLogInformation($"LogInformation");
        _logger.ZLogTrace($"This is a TRACE log.");
        _logger.ZLogDebug($"This is a DEBUG log.");
        _logger.ZLogWarning($"This is a WARNING log.");
        _logger.ZLogError($"This is an ERROR log.");
        _logger.ZLogCritical($"This is a CRITICAL log.");
    }
}