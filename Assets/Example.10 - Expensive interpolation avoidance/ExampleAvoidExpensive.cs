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


        _logger.ZLog(MessageLevel, $"Message with calculation {SuperExpensiveCalculation(CalculationTestValue)}", this); 
    }

    int SuperExpensiveCalculation(int a)
    {
        var obj = new GameObject($"Heavy calculation for {a}");
        return a;
    }
}