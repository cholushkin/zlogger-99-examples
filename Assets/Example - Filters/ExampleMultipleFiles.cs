// // --- Setup ---
// string infoLogPath = "C:/path/to/your/logs/application-info.log";
// string errorLogPath = "C:/path/to/your/logs/application-error.log";
//
// ILoggerFactory loggerFactory = LoggerFactory.Create(logging =>
// {
//     logging.SetMinimumLevel(LogLevel.Trace); // Allow all levels initially
//
//     // Configure INFO file logger (Information and above)
//     logging.AddZLoggerFile(infoLogPath, options =>
//     {
//         options.UsePlainTextFormatter();
//         options.Encoding = Encoding.UTF8;
//     });
//
//     // Configure ERROR file logger (Warning and above)
//     logging.AddZLoggerFile(errorLogPath, options =>
//     {
//         options.UsePlainTextFormatter();
//         options.Encoding = Encoding.UTF8;
//     });
//
//     // --- Apply Filters ---
//     // Filter for the INFO logger provider instance (based on file path)
//     logging.AddFilter<ZLoggerFileLoggerProvider>(
//         (provider) => provider.LogFilePath == infoLogPath, // Identify the correct provider
//         (category, level) => level >= LogLevel.Information // Only levels Information and higher
//     );
//
//     // Filter for the ERROR logger provider instance
//     logging.AddFilter<ZLoggerFileLoggerProvider>(
//         (provider) => provider.LogFilePath == errorLogPath,
//         (category, level) => level >= LogLevel.Warning // Only levels Warning and higher
//     );
//
// });
//
// // --- Usage ---
// ILogger<MyWorker> logger = loggerFactory.CreateLogger<MyWorker>();
// logger.ZLogDebug("This will NOT go to any file based on filters.");
// logger.ZLogInformation("Starting worker process."); // Goes to infoLogPath
// logger.ZLogWarning("Potential issue detected.");   // Goes to errorLogPath (and potentially infoLogPath if its filter allows Warning, which >= Information does)
// logger.ZLogError("Critical error occurred!");     // Goes to errorLogPath (and infoLogPath)
//
// // --- Important: Dispose factory at application shutdown ---
// // loggerFactory.Dispose();