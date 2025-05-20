using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger.Providers;
using System.IO;
using ZLogger;

[CreateAssetMenu(fileName = "FileLoggerConfig", menuName = "Logging/Providers/File Logger")]
public class FileLoggerConfig : LoggerProviderConfigBase
{
    public string FileName = "app.log";

    public override void Configure(ILoggingBuilder builder, string logDirectory)
    {
        if (!Enable) return;

        var fullPath = Path.Combine(logDirectory, FileName);

        builder.AddZLoggerFile(fullPath, options => { options.UsePlainTextFormatter(); });

        ApplyFilters<ZLoggerFileLoggerProvider>(builder);
    }
}