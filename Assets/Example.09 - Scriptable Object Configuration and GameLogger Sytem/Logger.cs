using System;
using Logging.Runtime;
using Microsoft.Extensions.Logging;


namespace Example09
{
    [Serializable]
    public class Logger
    {
        // - lazy interpolation
        // - local per logger level
        // - local enabled flag
        // - lazy get  instance of logmanager   
        public bool LocalIsEnabled;
        public string CategoryName;
        public LogLevel LocalLogLevel;
        protected ILogger? _logger;


        public virtual void Log(LogLevel logLevel, Func<string> messageFactory)
        {
            if (!LocalIsEnabled || logLevel < LocalLogLevel)
                return;

            LazyGetInstance();

            if (_logger != null && _logger.IsEnabled(logLevel)) // also checks provider-level filters
            {
                _logger.Log(logLevel, messageFactory());
            }
        }
    
        public void Info(Func<string> msg) => Log(LogLevel.Information, msg);
        public void Warn(Func<string> msg) => Log(LogLevel.Warning, msg);
        public void Error(Func<string> msg) => Log(LogLevel.Error, msg);
        public void Debug(Func<string> msg) => Log(LogLevel.Debug, msg);
        public void Trace(Func<string> msg) => Log(LogLevel.Trace, msg);

        protected void LazyGetInstance()
        {
            if(_logger == null)
                _logger = LogManager.Instance.CreateLogger(CategoryName);
        }
    }    
}
