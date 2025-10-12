using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger.Providers;
using System.IO;
using Logging.Config;
using ZLogger;

[CreateAssetMenu(fileName = "FileLoggerConfig", menuName = "Logging/Providers/File Logger")]
public class FileLoggerConfig : LoggerProviderConfigBase
{
    public string FileName = "app.log";

    public override void Configure(ILoggingBuilder builder, LoggerConfiguration config)
    {
        base.Configure(builder, config);
        
        if (!Enabled) return;

        var fullPath = Path.Combine(Application.persistentDataPath, FileName);

        builder.AddZLoggerFile(
            fullPath, options => { options.UsePlainTextFormatter(); });

        ApplyFilters<ZLoggerFileLoggerProvider>(builder, _config);
    }
}