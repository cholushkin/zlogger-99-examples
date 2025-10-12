using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using ILogger = Microsoft.Extensions.Logging.ILogger;


// AddZLoggerInMemory is designed for:
// - Enabling in-process consumption of log data, providing access to logs within the application itself.
// - Retrieving formatted log messages *after* ZLogger's processing and formatting have been applied.
// - Providing a mechanism to hook into ZLogger's log message processing pipeline, allowing custom actions on log events.
public class InMemoryExample : MonoBehaviour
{
    private ILoggerFactory _loggerFactory = null!;
    private ILogger _logger = null!;
    private List<string> _logMessages = new List<string>();

    void Start()
    {
        // 1. Configure ZLogger to use InMemory
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddZLoggerInMemory(processor =>
            {
                processor.MessageReceived += msg =>
                {
                    _logMessages.Add(msg); // Store messages in our list
                };
            });
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // 2. Create a logger
        _logger = _loggerFactory.CreateLogger<InMemoryExample>();

        // 3. Log some messages
        _logger.ZLogInformation($"This is an information message.");
        _logger.ZLogWarning($"This is a warning message.");

        // 4. Demonstrate accessing the messages
        DisplayInMemoryLogs();
    }

    void DisplayInMemoryLogs()
    {
        Debug.Log("[Logs from InMemory]");
        foreach (string log in _logMessages)
        {
            Debug.Log(log);
        }
    }
}