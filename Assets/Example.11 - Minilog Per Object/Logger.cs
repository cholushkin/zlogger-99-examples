// using System;
// using Microsoft.Extensions.Logging;
// using ZLogger;
//
// namespace Example11
// {
//     [Serializable]
//     public class LoggerWithMinilogSupport : Example09.Logger
//     {
//         public Minilog? Minilog;
//         public bool WriteToMiniLog;
//
//         /// <summary>
//         /// Logs via ZLogger and optionally forwards the message to Minilog.
//         /// </summary>
//         public override void Log(
//             LogLevel userLogLevel,
//             ref ZLoggerInterpolatedStringHandler message,
//             object? context = null,
//             string? memberName = null,
//             string? filePath = null,
//             int lineNumber = 0)
//         {
//             // Local filtering first
//             if (!LocalIsEnabled || userLogLevel < LocalLogLevel)
//                 return;
//
//             // Log via ZLogger (zero-allocation)
//             LazyGetInstance().ZLog(userLogLevel, ref message, context, memberName, filePath, lineNumber);
//
//             // Only allocate string if Minilog is enabled
//             if (Minilog != null && WriteToMiniLog)
//             {
//                 string msgString = message.GetState().ToString(); // Converts ZLoggerInterpolatedStringHandler to string
//                 AddMinilogMessage(userLogLevel, msgString);
//             }
//         }
//
//         private void AddMinilogMessage(LogLevel logLevel, string message)
//         {
//             Minilog!.AddMessage(logLevel, message); // Safe since null-checked above
//         }
//     }
// }