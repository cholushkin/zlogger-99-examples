using UnityEngine;
using Microsoft.Extensions.Logging;
using ZLogger;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ExampleAvoidExpensive : MonoBehaviour
{
    public LogLevel MinimumLevel = LogLevel.Warning;
    public LogLevel MessageLevel = LogLevel.Information;
    public int CalculationTestValue; 
    private ILogger _logger = null!;
    private ILoggerFactory _loggerFactory = null!;

    void Awake()
    {
        // Create the logger factory with ZLogger Unity debug output
        _loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.SetMinimumLevel(MinimumLevel); // only logs HighLogLevel and above

            logging.AddZLoggerUnityDebug(options =>
            {
                options.UsePlainTextFormatter();
                options.PrettyStacktrace = true;
            });
        });

        _logger = _loggerFactory.CreateLogger("ExampleAvoidExpensiveZLogger");


        _logger.ZLog(MessageLevel, $"ZLogger Message with calculation {SuperExpensiveCalculation($"ZLOGGER {CalculationTestValue}")}", this); 
        
        // this one will still do interpolation regardless of MinimumLevel. Don't use it!
        _logger.Log(MessageLevel, $"Logger Message with calculation {SuperExpensiveCalculation($"LOGGER {CalculationTestValue}")}", this);
    }

    string SuperExpensiveCalculation(string id)
    {
        var obj = new GameObject($"Heavy calculation for: '{id}'");
        return id;
    }
}