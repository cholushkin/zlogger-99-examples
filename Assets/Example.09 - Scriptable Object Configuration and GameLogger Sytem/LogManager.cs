using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Logging.Config;
using Microsoft.Extensions.Logging.Abstractions;
using ZLogger.Unity;

namespace Logging.Runtime
{
    public class LogManager : MonoBehaviour
    {
        [SerializeField] private LoggerConfiguration config;

        private static ILoggerFactory _loggerFactory = null!;
        public static LogManager Instance;

        [Tooltip("Master switch. When OFF, all ILogger instances are NullLogger and no providers initialize.")]
        public bool IsLoggingEnabled = true;

        void Awake()
        {
            Instance = this;

            // Global OFF → give out NullLogger and bail early
            if (!IsLoggingEnabled)
            {
                _loggerFactory = NullLoggerFactory.Instance;
                return;
            }

            Debug.Log($"Initializing LogManager using config: {config.name}");

            _loggerFactory = LoggerFactory.Create(builder =>
            {
                // Fallback when no configuration asset assigned
                if (config == null)
                {
                    Debug.LogError(
                        "[LogManager] LoggerConfiguration asset is missing or not assigned. " +
                        "Logging providers will not be initialized from asset; using Unity Console fallback.");
                    AddUnityConsoleProviderFallback(builder);
                    return;
                }

                // ===== Global rules =====
                // Hard floor (non-overridable clamp)
                builder.AddFilter((category, level) => level >= config.HardFloor);

                // Default soft minimum
                builder.SetMinimumLevel(config.DefaultMin);

                // ===== Select providers (Solo/Mute) =====
                var enabled = new List<LoggerConfiguration.ProviderConfiguration>(config.Providers ?? new());
                enabled.RemoveAll(p => p == null || p.Provider == null || p.Mute);

                if (enabled.Exists(p => p.Solo))
                    enabled.RemoveAll(p => !p.Solo);

                // ===== Configure each provider =====
                foreach (var p in enabled)
                {
                    // 1) Determine the *effective* floor for this provider.
                    //    Global HardFloor is absolute — cannot be lowered by provider configs.
                    var effectiveFloor = config.HardFloor;

                    // Provider HardFloor can only raise the threshold (never lower global).
                    if (p.HardFloor > effectiveFloor)
                        effectiveFloor = p.HardFloor;

                    // Provider DefaultMin can only raise (soft clamp) relative to global DefaultMin.
                    if (p.DefaultMin > config.DefaultMin && p.DefaultMin > effectiveFloor)
                        effectiveFloor = p.DefaultMin;

                    // 2) Apply the computed effective floor.
                    p.Provider.SetProviderWideFloor(builder, effectiveFloor);

                    // 3) Register provider + apply provider-scoped category filters
                    //    (base.Configure calls AddProvider() then ApplyProviderCategoryFilters())
                    p.Provider.Configure(builder, config);
                }
            });
        }

        public ILogger CreateLogger(string categoryName)
        {
            // For NullLoggerFactory: returns a NullLogger
            return _loggerFactory.CreateLogger(categoryName);
        }

        private void AddUnityConsoleProviderFallback(ILoggingBuilder builder)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            builder.SetMinimumLevel(LogLevel.Information);
#else
            builder.SetMinimumLevel(LogLevel.Warning);
#endif
            builder.AddZLoggerUnityDebug(options =>
            {
                options.UsePlainTextFormatter();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                options.PrettyStacktrace = true;
#else
                options.PrettyStacktrace = false;
#endif
            });
        }
    }
}