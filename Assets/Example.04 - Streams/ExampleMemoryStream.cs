using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ExampleMemoryStream : MonoBehaviour
{
    [Header("Logging Level")]
    public LogLevel minimumLevel = LogLevel.Debug;

    private ILogger _logger = null!;
    private ILoggerFactory _loggerFactory = null!;
    private MemoryStream? _logMemoryStream = null!;
    private StreamWriter? _streamWriter = null!;

    void Awake()
    {
        try
        {
            // --- Create Memory Stream and StreamWriter ---
            _logMemoryStream = new MemoryStream();
            _streamWriter = new StreamWriter(_logMemoryStream, Encoding.UTF8)
            {
                AutoFlush = true
            };

            // --- Initialize Logger Factory ---
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(minimumLevel);

                // Add ZLogger using the StreamWriter's BaseStream
                builder.AddZLoggerStream(_streamWriter.BaseStream, options =>
                {
                    options.UsePlainTextFormatter(formatter =>
                    {
                        formatter.SetPrefixFormatter($"{0} - ",
                            (in MessageTemplate template, in LogInfo info) => template.Format(info.Timestamp));
                    });
                });
            });

            _logger = _loggerFactory.CreateLogger<ExampleMemoryStream>();
            _logger.ZLogInformation($"Logger initialized using MemoryStream.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[{this.GetType().Name}] Failed to initialize ZLogger: {ex.Message}");
        }
    }

    private void Start()
    {
        _logger.ZLogInformation($"Application started.");
        _logger.ZLogDebug($"MemoryStream logging debug message.");
    }

    private void Update()
    {
        if (Time.frameCount % 100 == 0)
            _logger.ZLogInformation($"Frame: {Time.frameCount}, Random: {Random.value}");
    }

    private void OnDestroy()
    {
        // --- Read contents from MemoryStream and display them in the Unity Console ---
        if (_logMemoryStream != null)
        {
            try
            {
                _streamWriter?.Flush(); // ensure everything is written
                _logMemoryStream.Position = 0; // rewind the stream
                using var reader = new StreamReader(_logMemoryStream, Encoding.UTF8);
                string logContent = reader.ReadToEnd();

                Debug.Log($"[MemoryStream Log Dump]\n{logContent}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{this.GetType().Name}] Failed to read MemoryStream log: {ex.Message}");
            }

            _streamWriter?.Dispose();
            _logMemoryStream?.Dispose();
        }
        _loggerFactory?.Dispose();
    }
}
