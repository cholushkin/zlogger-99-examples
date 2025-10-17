// File: ILogManagerService.cs
using Microsoft.Extensions.Logging;

namespace Logging.Runtime
{
    public interface ILogManagerService
    {
        bool IsLoggingEnabled { get; }
        ILoggerFactory Factory { get; }
        ILogger CreateLogger(string categoryName);
    }
}