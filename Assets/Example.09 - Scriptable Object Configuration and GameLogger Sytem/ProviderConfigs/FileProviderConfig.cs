using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger;
using ZLogger.Providers;

namespace Logging.Config
{
    [CreateAssetMenu(
        fileName = "FileProviderConfig",
        menuName = "Logging/Providers/File Provider")]
    public class FileProviderConfig : LoggerProviderConfigBase
    {
        public enum Mode { SingleFile, RollingDaily }

        [Header("File Output")]
        [Tooltip("SingleFile writes to one file. RollingDaily creates a new file per day (and index).")]
        public Mode FileMode = Mode.RollingDaily;

        [Tooltip("For SingleFile: file path. For RollingDaily: target directory.")]
        public string PathOrDirectory = "Logs/app.log";

        [Tooltip("Base name for rolling files (ignored in SingleFile mode).")]
        public string BaseFileName = "app";

        [Header("Rolling Options")]
        [Tooltip("How often a new file is started when in RollingDaily mode.")]
        public RollingInterval RollingInterval = RollingInterval.Day;

        [Tooltip("Maximum size in KB before rolling to the next index.")]
        public int RollingSizeKB = 10 * 1024; // 10 MB

        [Header("Formatter")]
        [Tooltip("Use plain text instead of JSON.")]
        public bool UsePlainTextFormatter = true;

        // Diagnostics/UI only.
        public override Type ProviderType => typeof(ZLoggerFileLoggerProvider);

        // Register file/rolling-file provider (no filters here).
        protected override void AddProvider(ILoggingBuilder builder, LoggerConfiguration root)
        {
            switch (FileMode)
            {
                case Mode.SingleFile:
                {
                    var filePath = ResolveSingleFilePath(PathOrDirectory);
                    EnsureParentDirectory(filePath);

                    builder.AddZLoggerFile(filePath, o =>
                    {
                        if (UsePlainTextFormatter) o.UsePlainTextFormatter();
                        else o.UseJsonFormatter();
                    });
                    break;
                }

                case Mode.RollingDaily:
                {
                    var dir = ResolveDirectory(PathOrDirectory);
                    Directory.CreateDirectory(dir);

                    builder.AddZLoggerRollingFile(options =>
                    {
                        options.FilePathSelector = (dt, index) =>
                            Path.Combine(dir, $"{BaseFileName}-{dt:yyyyMMdd}-{index}.log");

                        options.RollingInterval = RollingInterval;
                        options.RollingSizeKB = RollingSizeKB;

                        if (UsePlainTextFormatter) options.UsePlainTextFormatter();
                        else options.UseJsonFormatter();
                    });
                    break;
                }
            }
        }

        // Provider-scoped category filters (typed).
        protected override void ApplyProviderCategoryFilters(
            ILoggingBuilder builder,
            IReadOnlyList<CategoryRule> categoryFilters)
        {
            if (categoryFilters == null || categoryFilters.Count == 0) return;

            foreach (var rule in categoryFilters)
            {
                if (rule == null) continue;
                var prefix = rule.CategoryPrefix ?? string.Empty;
                builder.AddFilter<ZLoggerFileLoggerProvider>(prefix, rule.MinLevel);
            }
        }

        // Provider-wide minimum level (applies to ALL categories for this provider).
        protected override void ApplyProviderWideFloor(ILoggingBuilder builder, LogLevel minLevel)
        {
            builder.AddFilter<ZLoggerFileLoggerProvider>(string.Empty, minLevel);
        }

        // --- helpers ---

        static string ResolveSingleFilePath(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
#if UNITY_EDITOR
                input = "Logs/app.log";
#else
                input = "app.log";
#endif
            }

#if UNITY_EDITOR
            if (!Path.IsPathRooted(input))
                input = Path.GetFullPath(Path.Combine(Application.dataPath, "..", input));
            return input;
#else
            if (!Path.IsPathRooted(input))
                input = Path.Combine(Application.persistentDataPath, input);
            return input;
#endif
        }

        static string ResolveDirectory(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                input = "Logs";

#if UNITY_EDITOR
            if (!Path.IsPathRooted(input))
                input = Path.GetFullPath(Path.Combine(Application.dataPath, "..", input));
            return input;
#else
            if (!Path.IsPathRooted(input))
                input = Path.Combine(Application.persistentDataPath, input);
            return input;
#endif
        }

        static void EnsureParentDirectory(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
        }
    }
}
