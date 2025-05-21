using System;
using Microsoft.Extensions.Logging;


namespace Example11
{
    [Serializable]
    public class Logger : Example09.Logger
    {
        public Minilog Minilog;
        public bool WriteToMiniLog;
        
        public override void Log(LogLevel logLevel, Func<string> messageFactory)
        {
            if (!LocalIsEnabled || logLevel < LocalLogLevel)
                return;

            LazyGetInstance();

            if (_logger != null && _logger.IsEnabled(logLevel)) // also checks provider-level filters
            {
                var message = messageFactory();
                _logger.Log(logLevel, message);
                if (Minilog != null && WriteToMiniLog)
                    AddMinilogMessage(logLevel, message);
            }
        }

        private void AddMinilogMessage(LogLevel logLevel, string message)
        {
            Minilog.AddMessage(logLevel, message);
        }
    }    
}
