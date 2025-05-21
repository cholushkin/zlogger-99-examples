using Microsoft.Extensions.Logging;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Logging.Config;

namespace Logging.Runtime
{
    public class LogManager : MonoBehaviour
    {
        [SerializeField]
        private LoggerConfiguration config;
        private static ILoggerFactory _loggerFactory = null!;
        public static LogManager Instance;

        void Awake()
        {
            Instance = this;
            if (_loggerFactory == null && config != null)
            {
                _loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.SetMinimumLevel(config.GlobalLogLevel);

                    foreach (var provider in config.Providers)
                    {
                        if (provider != null && provider.Enabled)
                        {
                            provider.Configure(builder, config);
                        }
                    }
                });
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggerFactory.CreateLogger(categoryName);
        }
    }
}