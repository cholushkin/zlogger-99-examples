using System.Collections.Generic;
using System.Threading; // Interlocked
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using UnityEngine;
using ZLogger.Unity;
using Logging.Config;
using NaughtyAttributes;
using ILogger = Microsoft.Extensions.Logging.ILogger;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Logging.Runtime
{
    [CreateAssetMenu(fileName = "LogManagerAsset", menuName = "Logging/Log Manager Asset")]
    public partial class LogManagerAsset : ScriptableObject
    {
        // --------------------------------------------------------
        // Singleton
        // --------------------------------------------------------
        private static LogManagerAsset _instance;

        // Backing field + public getter
        private static int _configVersion;
        public static int ConfigVersion => Volatile.Read(ref _configVersion);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticsOnPlay()
        {
            _instance = null;           // your existing singleton reset
            Volatile.Write(ref _configVersion, 0);
        }


        public static LogManagerAsset Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<LogManagerAsset>("LogManagerAsset");

                    if (_instance == null)
                    {
                        Debug.LogWarning("[LogManagerAsset] No asset found in Resources. Using fallback Unity Console logger.");
                        _instance = CreateInstance<LogManagerAsset>();
                        _instance._loggerFactory = CreateFallbackUnityConsoleFactory();
                        BumpVersion();
                        return _instance;
                    }

                    // Fresh runtime state whenever we (re)bind to the asset.
                    _instance._loggerFactory?.Dispose();
                    _instance._loggerFactory = null;
                }

                _instance.EnsureInitialized();
                return _instance;
            }
        }

        // --------------------------------------------------------
        // Config + State
        // --------------------------------------------------------
        [SerializeField] private LoggerConfiguration config;

        [Tooltip("Master switch. When OFF, all loggers are NullLogger.")]
        public bool IsLoggingEnabled = true;

        [System.NonSerialized] private ILoggerFactory _loggerFactory;

        // --------------------------------------------------------
        // Public API
        // --------------------------------------------------------
        public ILogger CreateLogger(string category)
        {
            // allow empty categories, but never null
            category ??= string.Empty;
            return Factory.CreateLogger(category);
        }

        /// <summary>
        /// Rebuild the logger factory NOW (e.g., from an Inspector button or menu item).
        /// Bumps ConfigVersion after a successful rebuild.
        /// </summary>
        [Button("Reload loggers and factory in memory")]
        public void ReloadNow() => Reinitialize();

        /// <summary>
        /// Optional: swap the configuration and rebuild immediately.
        /// </summary>
        public void ApplyConfiguration(LoggerConfiguration newConfig)
        {
            if (newConfig == null)
            {
                Debug.LogWarning("[LogManagerAsset] ApplyConfiguration called with null. Ignored.");
                return;
            }

            config = newConfig;
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
            Reinitialize();
        }

        // --------------------------------------------------------
        // Internals
        // --------------------------------------------------------
        private ILoggerFactory Factory
        {
            get
            {
                if (_loggerFactory == null) EnsureInitialized();
                return _loggerFactory ?? NullLoggerFactory.Instance;
            }
        }

        private void EnsureInitialized()
        {
            if (_loggerFactory != null)
                return;

            if (!IsLoggingEnabled)
            {
                _loggerFactory = NullLoggerFactory.Instance;
                BumpVersion(); // still signal a “new” config surface (everything muted)
                return;
            }

            Debug.Log($"[LogManagerAsset] Initializing logger factory using config: {config?.name ?? "<none>"}");

            _loggerFactory = LoggerFactory.Create(builder =>
            {
                if (config == null)
                {
                    AddUnityConsoleProviderFallback(builder);
                    return;
                }

                // Global floors
                builder.AddFilter((cat, lvl) => lvl >= config.HardFloor);
                builder.SetMinimumLevel(config.DefaultMin);

                // Enable providers (respect Solo/Mute)
                var enabled = new List<LoggerConfiguration.ProviderConfiguration>(config.Providers ?? new());
                enabled.RemoveAll(p => p == null || p.Provider == null || p.Mute);
                if (enabled.Exists(p => p.Solo))
                    enabled.RemoveAll(p => !p.Solo);

                foreach (var p in enabled)
                {
                    var floor = config.HardFloor;
                    if (p.HardFloor > floor) floor = p.HardFloor;
                    if (p.DefaultMin > config.DefaultMin && p.DefaultMin > floor) floor = p.DefaultMin;

                    p.Provider.SetProviderWideFloor(builder, floor);
                    p.Provider.Configure(builder, config);
                }
            });

            BumpVersion();
        }

        private void Reinitialize()
        {
            if (_loggerFactory != null)
            {
                Debug.Log("[LogManagerAsset] Reinitializing logger factory.");
                _loggerFactory.Dispose();
                _loggerFactory = null;
            }

            EnsureInitialized(); // will bump version as part of build
        }

        private static void BumpVersion()
        {
            Interlocked.Increment(ref _configVersion);
        }

        private static ILoggerFactory CreateFallbackUnityConsoleFactory()
        {
            return Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddFilter((cat, lvl) => lvl >= LogLevel.Information);
#else
                builder.SetMinimumLevel(LogLevel.Warning);
                builder.AddFilter((cat, lvl) => lvl >= LogLevel.Warning);
#endif
                builder.AddZLoggerUnityDebug(o =>
                {
                    o.UsePlainTextFormatter();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    o.PrettyStacktrace = true;
#else
                    o.PrettyStacktrace = false;
#endif
                });
            });
        }

        private void AddUnityConsoleProviderFallback(ILoggingBuilder builder)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            builder.SetMinimumLevel(LogLevel.Information);
#else
            builder.SetMinimumLevel(LogLevel.Warning);
#endif
            builder.AddZLoggerUnityDebug(o =>
            {
                o.UsePlainTextFormatter();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                o.PrettyStacktrace = true;
#else
                o.PrettyStacktrace = false;
#endif
            });
        }
    }
    
}
