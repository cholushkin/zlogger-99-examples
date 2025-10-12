using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;

// A custom log processor that sends logs to Unity's Debug.Log.
// This is a basic example to illustrate how to implement a custom log processor.
//
// Note: In real scenarios, using `builder.AddZLoggerUnityDebug()` is more efficient and feature-rich.
// This example is primarily for educational purposes to demonstrate how custom processors can be built.
public class UnityDebugLogProcessor : IAsyncLogProcessor
{
    public void Post(IZLoggerEntry log)
    {
        var formattedLog = log.ToString();
        log.Return(); // Always return the entry to the pool to avoid memory leaks.

        Debug.Log(formattedLog); // Output the formatted log to the Unity Console.
    }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}

public class ExampleLogProcessor : MonoBehaviour
{
    private ILoggerFactory _loggerFactory = null!;
    private ILogger _logger = null!;

    void Start()
    {
        // 1. Create an instance of your custom Log Processor
        var unityDebugProcessor = new UnityDebugLogProcessor();

        // 2. Configure ZLogger to use the LogProcessor
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddZLoggerLogProcessor(unityDebugProcessor);
            builder.SetMinimumLevel(LogLevel.Trace);
            
        });

        // 3. Create a logger
        _logger = _loggerFactory.CreateLogger<ExampleLogProcessor>();

        // 4. Log some messages
        _logger.ZLogInformation($"This is an information message using LogProcessor.");
        _logger.ZLogWarning($"This is a warning message via LogProcessor.");
        _logger.ZLogError($"This is an error message processed by LogProcessor.");

        // The logs will directly appear in the Unity Console thanks to our custom processor.
    }
}