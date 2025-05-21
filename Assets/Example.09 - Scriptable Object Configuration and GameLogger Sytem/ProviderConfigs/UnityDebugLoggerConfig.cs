using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger.Unity;

namespace Logging.Config
{
    [CreateAssetMenu(fileName = "UnityDebugLoggerConfig", menuName = "Logging/Providers/Unity Debug Logger")]
    public class UnityDebugLoggerConfig : LoggerProviderConfigBase
    {
        public bool PrettyStacktrace = true;

        public override void Configure(ILoggingBuilder builder, LoggerConfiguration config)
        {
            if (!Enabled) 
                return;
            
            base.Configure(builder, config);

            builder.AddZLoggerUnityDebug(options =>
            {
                options.UsePlainTextFormatter();
                options.PrettyStacktrace = PrettyStacktrace;
            });

            ApplyFilters<ZLoggerUnityDebugLoggerProvider>(builder, _config);
        }
    }
}