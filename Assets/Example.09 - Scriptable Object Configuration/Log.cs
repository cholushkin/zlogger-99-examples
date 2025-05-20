using Microsoft.Extensions.Logging;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Logging.Config;

namespace Logging.Runtime
{
    public class Log : MonoBehaviour
    {
        [SerializeField]
        private LoggerConfiguration config;

        private static ILoggerFactory _loggerFactory = null!;
        private ILogger _logger = null!;

        void Awake()
        {
            if (_loggerFactory == null && config != null)
            {
                var logDir = Application.persistentDataPath;

                _loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace); // Global default

                    foreach (var provider in config.Providers)
                    {
                        if (provider != null && provider.Enable)
                        {
                            provider.Configure(builder, logDir);
                        }
                    }
                });
            }

            _logger = _loggerFactory.CreateLogger("ScriptableLogger");
            _logger.LogInformation("Logger initialized via ScriptableObject configuration.");
        }
    }
}