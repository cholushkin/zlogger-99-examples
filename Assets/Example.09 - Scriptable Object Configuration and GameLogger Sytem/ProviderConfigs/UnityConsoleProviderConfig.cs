using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger;
using ZLogger.Unity;

namespace Logging.Config
{
    [CreateAssetMenu(
        fileName = "UnityConsoleProviderConfig",
        menuName = "Logging/Providers/Unity Console Provider")]
    public class UnityConsoleProviderConfig : LoggerProviderConfigBase
    {
        [Header("Formatter")]
        [Tooltip("Pretty-print stack traces in the Unity Console output.")]
        public bool PrettyStacktrace = true;

        [Tooltip("Use plain text instead of JSON.")]
        public bool UsePlainTextFormatter = true;

        [Tooltip("Timestamp format when timestamps are enabled by formatter options.")]
        public string TimestampFormat = "HH:mm:ss.fff";

        // Diagnostics/UI only.
        public override Type ProviderType => typeof(ZLoggerUnityDebugLoggerProvider);

        // Register Unity Console (ZLogger Unity Debug) provider.
        protected override void AddProvider(ILoggingBuilder builder, LoggerConfiguration root)
        {
            builder.AddZLoggerUnityDebug(options =>
            {
                if (UsePlainTextFormatter) options.UsePlainTextFormatter();
                else options.UseJsonFormatter();

                options.PrettyStacktrace = PrettyStacktrace;

                // If you later expose a toggle, you can wire TimestampFormat here.
                // options.TimestampFormat = TimestampFormat;
            });
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
                builder.AddFilter<ZLoggerUnityDebugLoggerProvider>(prefix, rule.MinLevel);
            }
        }

        // Provider-wide minimum level (applies to ALL categories for this provider).
        protected override void ApplyProviderWideFloor(ILoggingBuilder builder, LogLevel minLevel)
        {
            builder.AddFilter<ZLoggerUnityDebugLoggerProvider>(string.Empty, minLevel);
        }
    }
}
