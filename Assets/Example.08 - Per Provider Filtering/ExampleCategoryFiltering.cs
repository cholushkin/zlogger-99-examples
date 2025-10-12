using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using ZLogger.Unity;
using System.IO;
using ZLogger.Providers;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ExampleCategoryFiltering : MonoBehaviour
{
    private static ILoggerFactory _loggerFactory = null!;
    private ILogger _logger = null!;
    private ILogger _loggerMonster = null!;

    void Awake()
    {
        if (_loggerFactory == null)
        {
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                // Global default
                builder.SetMinimumLevel(LogLevel.Trace);

                // Unity Console: Allow everything down to Trace
                builder.AddZLoggerUnityDebug(options =>
                {
                    options.UsePlainTextFormatter();
                    options.PrettyStacktrace = true;
                });

                builder.AddFilter("Example.Monster", LogLevel.Warning);
            });
        }

        _logger = _loggerFactory.CreateLogger("Example");
        _loggerMonster = _loggerFactory.CreateLogger("Example.Monster");

        _logger.ZLogInformation($"spawn monster");
        _loggerMonster.ZLogInformation($"Argghh");
        _loggerMonster.ZLogWarning($"warn");
        _loggerMonster.ZLogError($"bug");
    }
}