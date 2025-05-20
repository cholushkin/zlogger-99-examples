using System;
using UnityEngine;
using Microsoft.Extensions.Logging;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;


public static class LoggerExtensions
{
    /// <summary>
    /// Logs a message at the specified log level, deferring the message generation
    /// until it's confirmed that the log level is enabled.
    /// This avoids expensive calculations if the log message is not going to be emitted.
    /// </summary>
    /// <param name="logger">The ILogger instance.</param>
    /// <param name="logLevel">The level at which to log the message.</param>
    /// <param name="messageFactory">A function that returns the log message string.
    /// This function will only be invoked if the logLevel is enabled.</param>
    public static void Log(this ILogger logger, LogLevel logLevel, Func<string> messageFactory)
    {
        // Check if the specified log level is enabled for this logger.
        // This is a cheap operation.
        if (logger.IsEnabled(logLevel))
        {
            // If enabled, then and only then, invoke the messageFactory to get the message string.
            // This ensures that any expensive calculations within messageFactory are only performed
            // when the log message will actually be processed.
            logger.Log(logLevel, messageFactory());
        }
    }

    // You can add more specific deferred logging methods if needed, e.g., LogInformationDeferred, LogErrorDeferred
    public static void LogInformation(this ILogger logger, Func<string> messageFactory)
    {
        logger.Log(LogLevel.Information, messageFactory);
    }

    public static void LogWarning(this ILogger logger, Func<string> messageFactory)
    {
        logger.Log(LogLevel.Warning, messageFactory);
    }

    public static void LogError(this ILogger logger, Func<string> messageFactory)
    {
        logger.Log(LogLevel.Error, messageFactory);
    }

    public static void LogCritical(this ILogger logger, Func<string> messageFactory)
    {
        logger.Log(LogLevel.Critical, messageFactory);
    }

    public static void LogDebug(this ILogger logger, Func<string> messageFactory)
    {
        logger.Log(LogLevel.Debug, messageFactory);
    }

    public static void LogTrace(this ILogger logger, Func<string> messageFactory)
    {
        logger.Log(LogLevel.Trace, messageFactory);
    }
}

public class ExampleAvoidExpensive : MonoBehaviour
{
    public LogLevel LowLogLevel;
    public LogLevel HighLogLevel;
    public LogLevel Between;

    private ILogger _logger = null!;
    private ILoggerFactory _loggerFactory = null!; // Store the factory instance


    void Awake()
    {
        _loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.SetMinimumLevel(HighLogLevel);

            logging.AddZLoggerUnityDebug(options =>
                {
                    options.UsePlainTextFormatter(); // Use plain text
                    options.PrettyStacktrace = true; // Format stack traces nicely
                }
            );
        });

        _logger = _loggerFactory.CreateLogger("ExampleAvoidExpensive"); // Category name

        _logger.Log(LowLogLevel,$"Low priority message {SuperExpensiveCalculation(41)}"); // Output of this will be cut, but the calculation is still happening on background
        _logger.Log(LowLogLevel, () => $"Low priority message {SuperExpensiveCalculation(42)}"); // Output of this will be cut and expensive calculation will be avoided
        _logger.Log(HighLogLevel,() => $"High priority message {SuperExpensiveCalculation(43)}"); 
    }

    int SuperExpensiveCalculation(int a)
    {
        var superExpensiveCalculationResult = a * 10;
        Debug.Log($"Super expensive calculation has been called for {a} and we have result {superExpensiveCalculationResult}");
        return superExpensiveCalculationResult;
    }
}