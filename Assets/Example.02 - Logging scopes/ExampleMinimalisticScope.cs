using System.IO;
using System.Text;
using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using Unity.VisualScripting;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ExampleMinimalisticScope : MonoBehaviour
{
    private ILogger _logger;
    private ILoggerFactory _loggerFactory; // Store the factory

    void Awake()
    {
        // Ensure log directory exists
        var logDirectory = Application.persistentDataPath; // Path.Combine is fine too, but often just the dir is needed
        var logFilePath = Path.Combine(logDirectory, "zlogger-log.txt");
        Debug.Log($"Log file path: {logFilePath}");

        // Create the factory and store it
        _loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Trace); // Log everything from Trace upwards

            // Configure Unity Debug Console output
            // Ensure you have ZLogger.Unity package installed
            logging.AddZLoggerUnityDebug(options =>
            {
                options.IncludeScopes = true;
                options.PrettyStacktrace = true;
                options.UsePlainTextFormatter(cfg =>
                {
                    cfg.SetPrefixFormatter($"{0}|{1}|", (in MessageTemplate template, in LogInfo info) =>
                    {
                        // --- Scope Formatting ---
                        string scopeString = ""; // Default
                        if (info.ScopeState != null && !info.ScopeState.IsEmpty)
                        {
                            var sb = new StringBuilder();
                            sb.Append('['); // Start bracket for scopes
                            var first = true;
                            // Access the structured properties
                            foreach (var kvp in info.ScopeState.Properties)
                            {
                                if (!first)
                                {
                                    sb.Append(", "); // Separator
                                }
                                sb.Append(kvp.Key);
                                sb.Append('=');
                                // Handle potential null value, call ToString() otherwise
                                sb.Append(kvp.Value?.ToString() ?? "null");
                                first = false;
                            }
                            sb.Append(']'); // End bracket
                            scopeString = sb.ToString();
                        }
                        // --- End Scope Formatting ---
                        template.Format(scopeString, info.LogLevel);
                    });
                });
            });

            // Configure File output
            logging.AddZLoggerFile(logFilePath, options =>
            {
                // File logging often includes scopes by default with structured logging (like JSON).
                // Explicitly configure if needed, e.g., for plain text:
                // options.UsePlainTextFormatter(formatterOptions => {
                //     formatterOptions.IncludeScopes = true;
                // });
                options.IncludeScopes = true;
                options.UseJsonFormatter(); // Example: Using JSON format for the file
            });
        });

        // Create the logger instance using the stored factory
        // Use the actual class name for the category for standard practice
        _logger = _loggerFactory.CreateLogger<ExampleMinimalisticScope>();
        // Or use a specific category name string: _logger = _loggerFactory.CreateLogger("MyExampleCategory");


        using (_logger.BeginScope("InitializationScope")) // Changed scope name for clarity
        {
            _logger.ZLogInformation($"Logger Initialized in Awake");
        }

        _logger.ZLogInformation($"Logger setup complete."); // Log outside scope
    }

    void Start()
    {
        // Check if logger is valid (it should be now)
        if (_logger == null)
        {
            Debug.LogError("Logger was not initialized correctly in Awake!");
            return;
        }

        // Example logging with scopes in Start
        using (_logger.BeginScope("scope-a")) // Use structured logging for scope values
        {
            _logger.ZLogInformation($"Processing order started.");
            using (_logger.BeginScope("scope-b")) // More specific scope name
            {
                _logger.ZLogDebug($"Querying orders: SELECT * FROM Orders WHERE Id = {0}", 32); // Example structured log
            }

            _logger.ZLogInformation($"Order processed successfully.");
        }
    }

    void OnDestroy()
    {
        // Dispose the factory when the GameObject is destroyed
        // This cleans up resources, like flushing file buffers.
        _logger?.ZLogInformation($"Disposing logger factory."); // Log disposal event if logger still valid
        _loggerFactory?.Dispose();
        _logger = null; // Help garbage collection
        _loggerFactory = null; // Help garbage collection
        Debug.Log("Logger factory disposed.");
    }
}