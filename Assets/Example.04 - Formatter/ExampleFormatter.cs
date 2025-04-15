// using System.IO;
// using System.Text;
// using UnityEngine;
// using ZLogger;
// using Microsoft.Extensions.Logging;
// using ZLogger.Unity;
// using ILogger = Microsoft.Extensions.Logging.ILogger;
//
// public class ExampleScope : MonoBehaviour
// {
//     private ILogger _logger;
//     private ILoggerFactory _loggerFactory; // Store the factory
//
//     void Awake()
//     {
//         // Create the factory and store it
//         _loggerFactory = LoggerFactory.Create(logging =>
//         {
//             logging.SetMinimumLevel(LogLevel.Trace); // Log everything from Trace upwards
//
//             // Configure Unity Debug Console output
//             logging.AddZLoggerUnityDebug(options =>
//             {
//                 options.IncludeScopes = true;
//                 options.PrettyStacktrace = true;
//                 // options.UsePlainTextFormatter(cfg =>
//                 // {
//                 //     cfg.SetPrefixFormatter($"{0}|{1}|", (in MessageTemplate template, in LogInfo info) =>
//                 //     {
//                 //         // --- Scope Formatting ---
//                 //         string scopeString = ""; // Default
//                 //         if (info.ScopeState != null && !info.ScopeState.IsEmpty)
//                 //         {
//                 //             var sb = new StringBuilder();
//                 //             sb.Append('['); // Start bracket for scopes
//                 //             var first = true;
//                 //             // Access the structured properties
//                 //             foreach (var kvp in info.ScopeState.Properties)
//                 //             {
//                 //                 if (!first)
//                 //                 {
//                 //                     sb.Append(", "); // Separator
//                 //                 }
//                 //                 sb.Append(kvp.Key);
//                 //                 sb.Append('=');
//                 //                 // Handle potential null value, call ToString() otherwise
//                 //                 sb.Append(kvp.Value?.ToString() ?? "null");
//                 //                 first = false;
//                 //             }
//                 //             sb.Append(']'); // End bracket
//                 //             scopeString = sb.ToString();
//                 //         }
//                 //         // --- End Scope Formatting ---
//                 //         template.Format(scopeString, info.LogLevel);
//                 //     });
//                 // });
//             });
//             
//             
//             // Configure json
//             logging.AddZLoggerUnityDebug(options =>
//             {
//                 options.IncludeScopes = true;
//                 options.UseJsonFormatter(); // Example: Using JSON format for the file
//             });
//         });
//
//         // Create the logger instance using the stored factory
//         // Use the actual class name for the category
//         _logger = _loggerFactory.CreateLogger<ExampleScope>();
//         // Or use a specific category name string: _logger = _loggerFactory.CreateLogger("MyExampleCategory");
//
//         using (_logger.BeginScope("InitializationScope")) // Changed scope name for clarity
//         {
//             _logger.ZLogInformation($"Logger setup complete."); 
//         }
//     }
//
//     void Start()
//     {
//         // Example logging with scopes in Start
//         using (_logger.BeginScope("order-processing")) // Use structured logging for scope values
//         {
//             _logger.ZLogInformation($"Processing order started.");
//             using (_logger.BeginScope("query")) // More specific scope name
//             {
//                 _logger.ZLogDebug($"Querying orders: SELECT * FROM Orders WHERE Id = {0}", 32); // Example structured log
//             }
//
//             _logger.ZLogInformation($"Order processed successfully.");
//         }
//     }
//
//     void OnDestroy()
//     {
//         _loggerFactory?.Dispose();
//         _logger = null; // Help garbage collection
//         _loggerFactory = null; // Help garbage collection
//     }
// }