using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using System.IO;
using System.IO.Compression;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ExampleZipStream : MonoBehaviour
{
    [Header("Logging Settings")]
    public string compressedLogFileName = "compressed_logs.gz";
    public LogLevel minimumLevel = LogLevel.Debug;

    private ILogger _logger = null!;
    private ILoggerFactory _loggerFactory = null!;
    private FileStream _fileStream = null!;
    private GZipStream _gzipStream = null!;

    void Awake()
    {
        try
        {
            // --- Create the compressed file stream ---
            string fullPath = Path.Combine(Application.persistentDataPath, compressedLogFileName);
            _fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            _gzipStream = new GZipStream(_fileStream, CompressionMode.Compress, leaveOpen: false);

            // --- Initialize logger factory with compressed stream ---
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(minimumLevel);
                builder.AddZLoggerStream(_gzipStream, options =>
                {
                    options.UsePlainTextFormatter();
                });
            });

            _logger = _loggerFactory.CreateLogger<ExampleZipStream>();
            _logger.ZLogInformation($"Compressed logger initialized.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[{this.GetType().Name}] Logger initialization failed: {ex.Message}");
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

            Debug.Log($"Compressed log written to: {Path.Combine(Application.persistentDataPath, compressedLogFileName)}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[{this.GetType().Name}] Cleanup error: {ex.Message}");
        }
    }
}
