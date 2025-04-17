using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using UnityEngine.Assertions;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;



public class ExampleLogToMiniConsole : MonoBehaviour
{
    public MiniConsole miniConsole = null!;
    private ILoggerFactory _loggerFactory = null!;
    private ILogger _logger = null!;

    void Start()
    {
        Assert.IsNotNull(miniConsole);
        // 1. Create an instance of your custom Log Processor
        IAsyncLogProcessor logProcessor = miniConsole;
        Debug.Assert(logProcessor != null);

        // 2. Configure ZLogger to use the LogProcessor
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddZLoggerLogProcessor(logProcessor!);
            builder.AddZLoggerUnityDebug();
            builder.SetMinimumLevel(LogLevel.Trace);
        });

        // 3. Create a logger
        _logger = _loggerFactory.CreateLogger<ExampleLogToMiniConsole>();

        // 4. Log some messages
        _logger.LogInformation("This is an information message using LogProcessor.");
        _logger.LogWarning("This is a warning message via LogProcessor.");
        _logger.LogError("This is an error message processed by LogProcessor.");

        // The logs will directly appear in the Unity Console thanks to our custom processor.
    }

    void OnDestroy()
    {
        _loggerFactory?.Dispose();
    }
}