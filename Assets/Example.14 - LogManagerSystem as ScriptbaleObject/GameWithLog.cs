using Microsoft.Extensions.Logging;
using NaughtyAttributes;
using UnityEngine;
using ZLogger;

public class GameWithLog : MonoBehaviour
{
    public Example.VersionAwareLogger.Logger Logger;
    
    [Button]
    void Start()
    {
        Logger.Instance().ZLog(Logger.Level(LogLevel.Trace), $"hi");
        
    }

}
