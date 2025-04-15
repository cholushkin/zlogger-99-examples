using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using System.IO; // Required for Path operations
using ZLogger.Providers; // Required for Encoding
using ILogger = Microsoft.Extensions.Logging.ILogger;

// Renamed class for clarity to match the intended functionality
public class ExampleDailyRollingJson : MonoBehaviour
{
    [Header("Log File Settings")]
    [Tooltip("Filename template (no subfolders). {0:yyyy-MM-dd} will be replaced by the date. {1} will be replaced by the index if file exceeds the size limit. File will be saved in Application.persistentDataPath.")]
    public string logFileNameTemplate = "Example.03.rolling-{0:yyyy-MM-dd}_{1}.json"; 

    [Header("Logging Level")]
    public LogLevel minimumLevel = LogLevel.Debug;
    
    [Tooltip("Maximum size per log file in Kilobytes before rolling (incrementing the index {1}).")]
    public int rollSizeKB = 1;

    public int logEveryNFrame = 10;
    
    // Internal fields
    private ILogger _logger = null!;
    private ILoggerFactory _loggerFactory = null!;

    void Awake()
    {
        // Combine persistentDataPath (a reliable writable location) with the template filename.
        string fullPathPattern = Path.Combine(Application.persistentDataPath, logFileNameTemplate);
        
        // Log the pattern being used (using Debug.Log as logger isn't ready yet)
        Debug.Log($"[{GetType().Name}] Initializing daily rolling log. Pattern: {fullPathPattern}");

        // --- Initialize Logger Factory ---
        try
        {
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(minimumLevel); // Set overall minimum level
                builder.AddZLoggerRollingFile(options =>
                {
                    options.FilePathSelector = (dt, index) => string.Format(fullPathPattern, dt, index);
                    options.RollingInterval = RollingInterval.Day;
                    options.RollingSizeKB = rollSizeKB;
                    options.UsePlainTextFormatter();
                });
            });

            // --- Create Logger Instance ---
            // Use the correct class name for the logger category
            _logger = _loggerFactory.CreateLogger<ExampleDailyRollingJson>();

            // Log initialization info using the created logger
            _logger.ZLogInformation($"-------------------------------------------");
            // Log the pattern being used, not a fixed path or mode
            _logger.ZLogInformation($"Logger initialized. Rolling Pattern: {fullPathPattern}");
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
        // Log every logEveryNFrame frames
        if (Time.frameCount % logEveryNFrame == 0)
            _logger.ZLogInformation($"Frame: {Time.frameCount}, RandomNumber:{Random.value}");
    }

    // Dispose the factory on destroy to flush logs
    void OnDestroy() => _loggerFactory.Dispose();
}