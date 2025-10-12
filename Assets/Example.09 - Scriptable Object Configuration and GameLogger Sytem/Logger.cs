using System;
using System.Runtime.CompilerServices;
using Logging.Runtime;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace Example09
{
    [Serializable]
    public class Logger
    {
        public bool LocalIsEnabled = true;
        public string CategoryName = "";
        public LogLevel LocalLogLevel = LogLevel.Information;

        protected ILogger? _logger;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected ILogger LazyGetInstance()
            => _logger ??= LogManager.Instance.CreateLogger(CategoryName);

        private const LogLevel None = (LogLevel)(-1);

        // 1️⃣ Return actual ILogger instance
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ILogger Instance()
            => LazyGetInstance();

        // 2️⃣ Local log-level check: returns valid level or sentinel
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LogLevel Level(LogLevel requestedLevel)
        {
            if (!LocalIsEnabled || requestedLevel < LocalLogLevel)
                return None; // disables ZLogger handler creation
            return requestedLevel;
        }
    }
}