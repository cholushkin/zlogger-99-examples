using UnityEngine;

public class UnityLogExample : MonoBehaviour
{
    private void Start()
    {
        // ---------- BASIC LOGGING ----------
        Debug.Log("Hello, World!");                                 // Regular log
        Debug.Log("Hello, LogExample!", this);               // Regular log with context
        Debug.Log("Game started at: " + Time.time);                 // Concatenated
        Debug.LogFormat("Position: ({0}, {1})", 10, 20);            // Formatted log

        // ---------- WARNING & ERROR ----------
        Debug.LogWarning("This is a warning!");
        Debug.LogWarningFormat("Low health: {0}%", 15);

        Debug.LogError("This is an error!");
        Debug.LogErrorFormat("Critical error: {0}", "NullReferenceException");

        // ---------- ASSERTIONS ----------
        Debug.Assert(1 + 1 == 2, "Math is broken!");
        Debug.Assert(gameObject != null, "GameObject is missing!");

        // ---------- EXCEPTIONS ----------
        try
        {
            throw new System.Exception("Something bad happened!");
        }
        catch (System.Exception e)
        {
            Debug.LogException(e); // Logs full stack trace
        }

        // ---------- CONDITIONAL LOGGING ----------
#if UNITY_EDITOR
        Debug.Log("Only logs in the editor.");
#endif

        // ---------- DRAWING IN THE EDITOR ----------
        Debug.DrawLine(Vector3.zero, Vector3.one * 10, Color.red, 5.0f); // Visual debugging

        // ---------- ADVANCED LOGGER ----------
        var logger = new Logger(Debug.unityLogger.logHandler);
        logger.logEnabled = true;
        logger.filterLogType = LogType.Warning; // Will only log warnings and errors

        logger.Log(LogType.Log, "This will NOT appear.");
        logger.Log(LogType.Warning, "This is a filtered warning.");
        logger.Log(LogType.Error, "This is a filtered error.");

        // ---------- CUSTOM TAG ----------
        Debug.unityLogger.Log("SYSTEM", "Tagged log message.");

    }
}
