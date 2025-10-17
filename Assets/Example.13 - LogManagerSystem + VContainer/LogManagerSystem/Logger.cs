// File: Logger.cs
using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Logging.Runtime
{
    /// <summary>
    /// Serializable helper you can put on MonoBehaviours.
    /// It gates logging locally and forwards to a global ILoggerFactory provided via Bind().
    /// </summary>
    [Serializable]
    public class Logger
    {
        public bool LocalIsEnabled = true;
        public string CategoryName = "";
        public LogLevel LocalLogLevel = LogLevel.Information;

        private ILoggerFactory _factory = NullLoggerFactory.Instance;
        [NonSerialized] private ILogger? _logger;

        private const LogLevel None = (LogLevel)(-1);

        /// <summary>Call this from your MonoBehaviour’s [Inject] Construct to wire the global factory.</summary>
        public void Bind(ILoggerFactory factory)
        {
            _factory = factory ?? NullLoggerFactory.Instance;
            _logger = null; // reset so we re-create with the (possibly) new factory
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ILogger LazyGetInstance()
            => _logger ??= _factory.CreateLogger(string.IsNullOrEmpty(CategoryName) ? "Unknown" : CategoryName);

        /// <summary>Return the actual ILogger instance (may be NullLogger if not bound or disabled globally).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ILogger Instance() => LazyGetInstance();

        /// <summary>
        /// Local log-level gate. Returns <see cref="None"/> if the requested level should be skipped locally.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LogLevel Level(LogLevel requestedLevel)
        {
            if (!LocalIsEnabled || requestedLevel < LocalLogLevel)
                return None;
            return requestedLevel;
        }

        // Optional convenience:
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Should(LogLevel requestedLevel) => Level(requestedLevel) != None;
    }
}
