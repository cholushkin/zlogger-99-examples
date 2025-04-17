using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger;

public class MiniConsole : MonoBehaviour, IAsyncLogProcessor
{
    public List<string> messages = new ();
    public LogLevel minLogLevel;
    
    public void Post(IZLoggerEntry log)
    {
        if (log.LogInfo.LogLevel < minLogLevel)
        {
            log.Return();
            return;
        }
        var formattedLog = log.ToString();
        log.Return(); // Important: Return the entry to the pool

        messages.Add(formattedLog);
    }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}
