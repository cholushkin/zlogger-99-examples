using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ExamplePlainTextFormatter : MonoBehaviour
{
    private ILogger _logger = null!;
    private ILoggerFactory _loggerFactory = null!;

    void Awake()
    {
        _loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Trace);

            logging.AddZLoggerUnityDebug(options =>
            {
                options.IncludeScopes = true;
                options.PrettyStacktrace = true;

                options.UsePlainTextFormatter(cfg =>
                {
                    cfg.SetPrefixFormatter($"{0}", (in MessageTemplate template, in LogInfo info) =>
                    {
                        string scopeString = ScopeFormatter.FormatScope(info.ScopeState);
                        template.Format(scopeString);
                    });
                });
            });
        });

        _logger = _loggerFactory.CreateLogger<ExamplePlainTextFormatter>();
    }

    void Start()
    {
        using (_logger.BeginScope("root-scope"))
        using (_logger.BeginScope("order-processing"))
        using (_logger.BeginScope(new { OrderId = 42 }))
        {
            _logger.ZLogWarning($"Order received.");
        }
    }
}