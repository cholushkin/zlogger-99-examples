// File: LogManagerService.cs
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using VContainer.Unity;
using Logging.Config;
using UnityEngine;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Logging.Runtime
{
    /// <summary>
    /// Pure DI service (no MonoBehaviour). Built and owned by VContainer.
    /// </summary>
    public sealed class LogManagerService : ILogManagerService, IStartable, IDisposable
    {
        private readonly LoggerConfiguration _config;
        private readonly bool _isLoggingEnabled;

        public ILoggerFactory Factory { get; private set; } = NullLoggerFactory.Instance;
        
        // Master switch. When OFF, all ILogger instances are NullLogger and no providers initialize
        public bool IsLoggingEnabled => _isLoggingEnabled;

        public LogManagerService(LoggerConfiguration config, bool isLoggingEnabled = true)
        {
            _config = config;
            _isLoggingEnabled = isLoggingEnabled;
        }

        // Called by VContainer when the scope is built
        public void Start()
        {
            if (!_isLoggingEnabled)
            {
                Factory = NullLoggerFactory.Instance;
                return;
            }
            
            Debug.Log($"[LogManagerService] Initializing using config: {(_config ? _config.name : "<null>")}");



            Factory = LoggerFactory.Create(builder =>
            {
                if (_config == null)
                {
                    Debug.LogError(
                        "[LogManagerService] LoggerConfiguration asset is missing or not assigned. " +
                        "Logging providers will not be initialized from asset; using Unity Console fallback.");
                    AddUnityConsoleProviderFallback(builder);
                    return;
                }

                // ===== Global rules =====
                // Hard floor (non-overridable clamp)
                builder.AddFilter((category, level) => level >= _config.HardFloor);

                // Default soft minimum
                builder.SetMinimumLevel(_config.DefaultMin);

                // ===== Select providers (Solo/Mute) =====
                var enabled = new List<LoggerConfiguration.ProviderConfiguration>(_config.Providers ?? new());
                enabled.RemoveAll(p => p == null || p.Provider == null || p.Mute);

                if (enabled.Exists(p => p.Solo))
                    enabled.RemoveAll(p => !p.Solo);

                // ===== Configure each provider =====
                foreach (var p in enabled)
                {
                    // 1) Determine the *effective* floor for this provider.
                    //    Global HardFloor is absolute — cannot be lowered by provider configs.
                    var effectiveFloor = _config.HardFloor;

                    // Provider HardFloor can only raise the threshold (never lower global).
                    if (p.HardFloor > effectiveFloor)
                        effectiveFloor = p.HardFloor;

                    // Provider DefaultMin can only raise (soft clamp) relative to global DefaultMin.
                    if (p.DefaultMin > _config.DefaultMin && p.DefaultMin > effectiveFloor)
                        effectiveFloor = p.DefaultMin;

                    // 2) Apply the computed effective floor.
                    p.Provider.SetProviderWideFloor(builder, effectiveFloor);

                    // 3) Register provider + apply provider-scoped category filters
                    //    (base.Configure calls AddProvider() then ApplyProviderCategoryFilters())
                    p.Provider.Configure(builder, _config);
                }
            });
        }

        public ILogger CreateLogger(string categoryName) => Factory.CreateLogger(categoryName); // For NullLoggerFactory: returns a NullLogger

        public void Dispose() => Factory?.Dispose();

        private static void AddUnityConsoleProviderFallback(ILoggingBuilder builder)
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
