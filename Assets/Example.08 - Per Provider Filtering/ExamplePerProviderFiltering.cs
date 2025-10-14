using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using ZLogger.Unity;
using System.IO;
using ZLogger.Providers;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ExamplePerProviderFiltering : MonoBehaviour
{
    private static ILoggerFactory _loggerFactory = null!;
    private ILogger _logger = null!;

    void Awake()
    {
        if (_loggerFactory == null)
        {
            string logFilePath = Path.Combine(Application.persistentDataPath, "app.log");

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

                builder.AddFilter<ZLoggerUnityDebugLoggerProvider>((category, level) => level >= LogLevel.Trace);

                // File logger: Allow Error and above only
                builder.AddZLoggerFile(logFilePath, options => { options.UsePlainTextFormatter(); });
                builder.AddFilter<ZLoggerFileLoggerProvider>((category, level) => level >= LogLevel.Error);
            });
        }

        _logger = _loggerFactory.CreateLogger("MyGameLogger");

        _logger.ZLogTrace($"TRACE: Visible only in Unity Console.");
        _logger.ZLogDebug($"DEBUG: Visible only in Unity Console.");
        _logger.ZLogInformation($"INFO: Visible only in Unity Console.");
        _logger.ZLogWarning($"WARNING: Visible in Unity Console.");
        _logger.ZLogError($"CRITICAL: Visible in both Unity Console and log file.");
        _logger.ZLogCritical($"CRITICAL: Visible in both Unity Console and log file.");

        Debug.Log($"Writing log to {Application.persistentDataPath}");
    }
}