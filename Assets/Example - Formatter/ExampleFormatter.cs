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


// todo: separate method which gets span and return string of scopes

//
// using UnityEngine;
// using ZLogger;
// using Microsoft.Extensions.Logging;
// using System.IO;
// using System.Collections.Generic; // Needed for List
// using ILogger = Microsoft.Extensions.Logging.ILogger;
// using ZLogger.Formatters; // Required for IZLoggerFormatter, ILogScopeState
//
// public class ExampleScopePrefix : MonoBehaviour
// {
//     // Enum definition for the Inspector dropdown
//     public enum FileWriteMode { Append, Rewrite }
//
//     [Header("Log File Settings")]
//     [Tooltip("Filename only (no subfolders). File will be saved in Application.persistentDataPath.")]
//     public string logFileName = "ScopePrefixLog.log";
//
//     [Tooltip("Append: Add to existing log file.\nRewrite: Delete existing log file on start.")]
//     public FileWriteMode writeMode = FileWriteMode.Append;
//
//     [Header("Logging Level")]
//     public LogLevel minimumLevel = LogLevel.Debug;
//
//     private ILogger _logger = null!;
//     private ILoggerFactory _loggerFactory = null!;
//     private string _fullLogPath = null;
//
//     // Helper method from previous example
//     private string PrepareLogFileAndGetPath(string fileNameOnly, FileWriteMode mode)
//     {
//         string fullPath = Path.Combine(Application.persistentDataPath, fileNameOnly);
//         if (mode == FileWriteMode.Rewrite) { /* ... delete logic ... */ }
//         // Simplified delete logic for brevity:
//         if (mode == FileWriteMode.Rewrite && File.Exists(fullPath)) try { File.Delete(fullPath); } catch { /* ignored */ }
//         return fullPath;
//     }
//
//     void Awake()
//     {
//         _fullLogPath = PrepareLogFileAndGetPath(logFileName, writeMode);
//
//         try
//         {
//             _loggerFactory = LoggerFactory.Create(builder =>
//             {
//                 builder.SetMinimumLevel(minimumLevel);
//                 builder.AddZLoggerFile(_fullLogPath, options =>
//                 {
//                     // --- Configure Plain Text Formatter ---
//                     options.UsePlainTextFormatter(formatterOpt =>
//                     {
//                         // Define the overall prefix structure, using {scope} as a placeholder
//                         // The content of {scope} will be generated by SetScopeFormatter below.
//                         formatterOpt.DefaultPrefixFormat = "{scope}{timestamp:HH:mm:ss} [{logLevel}] ";
//
//                         // --- Define how scopes are formatted ---
//                         formatterOpt.SetScopeFormatter((writer, scopeState) =>
//                         {
//                             // If there are no scopes, do nothing.
//                             if (scopeState == null) return;
//
//                             // Scopes are typically nested like a linked list (inner-most first).
//                             // To print outer-most first, collect them and reverse.
//                             var scopes = new List<object?>();
//                             var current = scopeState;
//                             while (current != null)
//                             {
//                                 // Try to get the actual State object passed to BeginScope.
//                                 // This might require accessing internal properties if not directly exposed.
//                                 // We use a common pattern here; adjust if ZLogger's structure differs.
//                                 object? scopeObj = current; // Default to the node itself
//                                 if (current is ZLogger.Internal.LogScopeState internalState)
//                                 {
//                                     // Accessing internal state - might change between ZLogger versions!
//                                     scopeObj = internalState.State;
//                                 }
//
//                                 scopes.Add(scopeObj);
//                                 current = current.Parent; // Move up the scope chain
//                             }
//
//                             // Reverse the list to get outer-most scope first
//                             scopes.Reverse();
//
//                             // Write each scope formatted as [ScopeString]
//                             foreach (var scope in scopes)
//                             {
//                                 writer.Write('[');
//                                 // Use ToString() for the scope object's representation.
//                                 // Ensure your scoped objects have meaningful ToString() overrides if needed.
//                                 writer.Write(scope?.ToString() ?? "null");
//                                 writer.Write(']');
//                             }
//
//                             // Add a space after the scopes if any were written
//                             writer.Write(' ');
//                         }); // End SetScopeFormatter
//                     }); // End UsePlainTextFormatter
//                 }); // End AddZLoggerFile
//             }); // End LoggerFactory.Create
//
//             _logger = _loggerFactory.CreateLogger<ExampleScopePrefix>();
//             _logger.ZLogInformation($"Logger initialized. Mode: {writeMode}. Path: {_fullLogPath}");
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError($"[{this.GetType().Name}] Failed to initialize ZLogger: {ex.Message}");
//             _logger = null; _loggerFactory = null;
//         }
//     }
//
//     void Start()
//     {
//         if (_logger == null) return;
//
//         _logger.ZLogInformation("Logging without scope.");
//
//         // Example with nested scopes
//         using (_logger.BeginScope("OuterScope"))
//         {
//             _logger.ZLogInformation("Inside outer scope.");
//
//             var playerState = new { PlayerId = 101, Name = "Hero" };
//             using (_logger.BeginScope(playerState)) // Structured scope
//             {
//                 _logger.ZLogWarning("Inside outer and player scope.");
//
//                 using (_logger.BeginScope("InnerMostAction")) // String scope
//                 {
//                     _logger.ZLogError("Deeply nested log.");
//                 } // InnerMostAction scope ends
//             } // playerState scope ends
//
//              _logger.ZLogInformation("Back in outer scope only.");
//         } // OuterScope scope ends
//
//         _logger.ZLogInformation("Logging after all scopes.");
//     }
//
//     void OnDestroy() => _loggerFactory?.Dispose();
// }