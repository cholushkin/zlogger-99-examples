using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using System.IO; // Required for File and Path operations
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ExampleFileProviderBasic : MonoBehaviour
{
    // Enum definition for the Inspector dropdown
    public enum FileWriteMode
    {
        Append, // Add to the end of the file if it exists
        Rewrite // Delete the existing file and start fresh
    }

    [Header("Log File Settings")]
    [Tooltip("Filename only (no subfolders). File will be saved in Application.persistentDataPath.")]
    public string logFileName = "MyGameLog.log"; // Filename only, no path

    [Tooltip("Append: Add to existing log file.\nRewrite: Delete existing log file on start.")]
    public FileWriteMode writeMode = FileWriteMode.Append; // Default to Append

    [Header("Logging Level")]
    public LogLevel minimumLevel = LogLevel.Debug;

    // Internal fields
    private ILogger _logger = null!;
    private ILoggerFactory _loggerFactory = null!;
    private string? _fullLogPath = null; // To store the calculated full path


    private string PrepareLogFileAndGetPath(string fileNameOnly, FileWriteMode mode)
    {
        // Combine persistentDataPath with the filename. persistentDataPath is a reliable writable location.
        string fullPath = Path.Combine(Application.persistentDataPath, fileNameOnly);

        // Handle Rewrite Logic: Delete the file if it exists and mode is Rewrite.
        if (mode == FileWriteMode.Rewrite)
        {
            try
            {
                if (File.Exists(fullPath))
                {
                    // Using Debug.Log here since the logger isn't initialized yet.
                    Debug.Log($"[{this.GetType().Name}] Rewrite mode enabled. Deleting existing log file: {fullPath}");
                    File.Delete(fullPath);
                }
            }
            catch (System.Exception ex)
            {
                // Log error if deletion fails (e.g., permissions issue) but continue.
                Debug.LogError($"[{this.GetType().Name}] Failed to delete log file for rewrite: {ex.Message}. Path: {fullPath}");
            }
        }
        return fullPath;
    }

    void Awake()
    {
        // --- Prepare Log File Path (and potentially delete existing) ---
        _fullLogPath = PrepareLogFileAndGetPath(logFileName, writeMode);
        Debug.Log($"Working with file: {_fullLogPath}");

        // --- Initialize Logger Factory ---
        // Use a try-catch block for robustness during initialization
        try
        {
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(minimumLevel); // Set overall minimum level

                // Add ZLogger File Logger using the determined full path
                builder.AddZLoggerFile(_fullLogPath, options =>
                {
                    // Add timestamp 
                    options.UsePlainTextFormatter(formatter =>
                    {
                        formatter.SetPrefixFormatter($"{0} - ", (in MessageTemplate template, in LogInfo info) => template.Format(info.Timestamp));
                    });
                });
            });

            _logger = _loggerFactory.CreateLogger<ExampleFileProviderBasic>();

            // Log initialization info, including the mode and path
            _logger.ZLogInformation($"-------------------------------------------");
            _logger.ZLogInformation($"Logger initialized. Mode: {writeMode}. Path: {_fullLogPath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[{this.GetType().Name}] Failed to initialize ZLogger: {ex.Message}");
        }
    }

    private void Start()
    {
        _logger.ZLogInformation($"Application started.");
        _logger.ZLogDebug($"Debug information for diagnostics.");
    }

    private void Update()
    {
        // Log every 100 frames (example)
        if (Time.frameCount % 100 == 0)
            _logger.ZLogInformation($"Frame: {Time.frameCount}, RandomNumber:{Random.value}");
    }

    // Dispose the factory on destroy to flush logs
    void OnDestroy() => _loggerFactory?.Dispose();
}