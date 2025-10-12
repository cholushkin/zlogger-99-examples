using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using System.IO;
using System.IO.Compression;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ExampleZipStream : MonoBehaviour
{
    [Header("Logging Settings")]
    [Tooltip("Compressed log file name (will be created as .gz)")]
    public string compressedLogFileName = "compressed_logs.gz";

    [Tooltip("Minimum logging level to record.")]
    public LogLevel minimumLevel = LogLevel.Debug;

    [Tooltip("If true, writes to Unity project's Logs/ folder instead of persistent data path. Only in EDITOR")]
    public bool writeToProjectLogFolder = false;

    private ILogger _logger = null!;
    private ILoggerFactory _loggerFactory = null!;
    private FileStream _fileStream = null!;
    private GZipStream _gzipStream = null!;
    private string _finalPath = string.Empty;

    void Awake()
    {
        try
        {
            // --- Determine destination path ---
            if (writeToProjectLogFolder)
            {
#if UNITY_EDITOR
                // In Editor, go up from Assets to project root and add "Logs/"
                string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                string logDir = Path.Combine(projectRoot, "Logs");
                Directory.CreateDirectory(logDir);
                _finalPath = Path.Combine(logDir, compressedLogFileName);
#else
                // In builds, fall back to persistent data
                _finalPath = Path.Combine(Application.persistentDataPath, compressedLogFileName);
#endif
            }
            else
            {
                // Default persistent path
                _finalPath = Path.Combine(Application.persistentDataPath, compressedLogFileName);
            }

            // --- Create file and wrap it in a GZip stream ---
            _fileStream = new FileStream(_finalPath, FileMode.Create, FileAccess.Write, FileShare.None);
            _gzipStream = new GZipStream(_fileStream, CompressionMode.Compress, leaveOpen: false);

            // --- Configure ZLogger factory ---
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(minimumLevel);
                builder.AddZLoggerStream(_gzipStream, options =>
                {
                    options.UsePlainTextFormatter();
                });
            });

            _logger = _loggerFactory.CreateLogger<ExampleZipStream>();
            _logger.ZLogInformation($"Compressed logger initialized at: {_finalPath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[{nameof(ExampleZipStream)}] Logger initialization failed: {ex.Message}");
        }
    }

    void Start()
    {
        _logger.ZLogInformation($"Application started with GZipStream logging.");
    }

    void Update()
    {
        if (Time.frameCount % 100 == 0)
            _logger.ZLogInformation($"Frame: {Time.frameCount}, DeltaTime: {Time.deltaTime}");
    }

    void OnDestroy()
    {
        try
        {
            _loggerFactory?.Dispose();
            _gzipStream?.Dispose();
            _fileStream?.Dispose();

            Debug.Log($"Compressed log written to: {_finalPath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[{nameof(ExampleZipStream)}] Cleanup error: {ex.Message}");
        }
    }
}
