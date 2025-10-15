using Microsoft.Extensions.Logging;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using ZLogger;

public class Monster : MonoBehaviour
{
    public Example09.Logger Logger;
    public int MonsterId;
    [FormerlySerializedAs("hitPoints")] public int HitPoints;

    void Start()
    {
        // ❌ We can’t use this simpler form:
        // Logger.Log(LogLevel.Information, $"Initializing Monster");
        //
        // Because Unity currently runs on a .NET Standard 2.1 / .NET Framework–based runtime
        // (not full .NET 6 or higher). ZLogger’s zero-allocation logging relies on
        // the C# 10 feature “interpolated string handlers” together with the
        // [InterpolatedStringHandlerArgument] attribute for compile-time code generation.
        // That attribute and its compiler behavior are only available in .NET 6+.
        //
        // Without .NET 6, implementing a proper wrapper that preserves
        // ZLogger’s zero-alloc behavior isn’t possible — the compiler won’t
        // recognize the attribute and will treat the interpolated string as a normal one,
        // causing allocations. Therefore, we call ZLogger directly like this instead:

        Logger.Instance().ZLog(Logger.Level(LogLevel.Information), $"Initializing Monster");

        // todo: wait for .NET 6 support in Unit and then rewrite to the signature like this instead:
        //Logger.Log(LogLevel.Information, $"Initializing Monster");
    }

    // ─────────────────────────────────────────────
    // Context Menu actions (right–click the component header)
    // ─────────────────────────────────────────────

    [Button("Log Trace")]
    void CM_LogTrace()
    {
        Debug.Log($"ENABLED? Trace={Logger.Instance().IsEnabled(LogLevel.Trace)}, " +
                  $"Debug={Logger.Instance().IsEnabled(LogLevel.Debug)}, " +
                  $"Info={Logger.Instance().IsEnabled(LogLevel.Information)}, " +
                  $"Warn={Logger.Instance().IsEnabled(LogLevel.Warning)}, " +
                  $"Error={Logger.Instance().IsEnabled(LogLevel.Error)}");

        
        if(Logger.Instance().IsEnabled(LogLevel.Trace))
            Debug.Log("enabled check is true");
        Logger.Instance().ZLog(
            Logger.Level(LogLevel.Trace),
            $"[Trace] Monster '{MonsterId}' tick at {Time.frameCount}",
            this);
    }

    [Button("Log Debug (skips heavy work when filtered)")]
    void CM_LogDebugHeavy()
    {
        // If current minimum level is ≥ Information, ZLogger’s handler will short-circuit
        // and SuperExpensiveComputation() will NOT execute.
        Logger.Instance().ZLog(
            Logger.Level(LogLevel.Debug),
            $"[Debug] '{MonsterId}' debug stats: {SuperExpensiveComputation()}",
            this);
    }

    [Button("Log Information")]
    void CM_LogInformation()
    {
        Logger.Instance().ZLog(
            Logger.Level(LogLevel.Information),
            $"[Info] '{MonsterId}' patrols at {transform.position}",
            this);
    }

    [Button("Log Warning (took damage)")]
    void CM_LogWarning()
    {
        int damage = Random.Range(5, 20);
        HitPoints = Mathf.Max(0, HitPoints - damage);

        Logger.Instance().ZLog(
            Logger.Level(LogLevel.Warning),
            $"[Warn] '{MonsterId}' took {damage} dmg → HP={HitPoints}",
            this);
    }

    [Button("Log Error (simulate failure)")]
    void CM_LogError()
    {
        try
        {
            SimulatePathfindingFailure();
        }
        catch (System.Exception ex)
        {
            // Plain ZLogError is fine too; showing explicit level variant for symmetry with your wrapper
            Logger.Instance().ZLog(
                Logger.Level(LogLevel.Error),
                $"[Error] '{MonsterId}' pathfinding failed on node #{Random.Range(100, 999)}. Exception={ex.Message}",
                this);
        }
    }

    [Button("Log Critical (HP=0)")]
    void CM_LogCritical()
    {
        HitPoints = 0;
        Logger.Instance().ZLog(
            Logger.Level(LogLevel.Critical),
            $"[Critical] '{MonsterId}' is dead. Despawning entity.",
            this);
    }

    // Helpers
    string SuperExpensiveComputation()
    {
        // Simulate costly string/data build that should be skipped when level is filtered out.
        // Do not prebuild strings outside ZLog; let the interpolated handler decide.
        var temp = new GameObject($"tmp-{MonsterId}-{Time.frameCount}");
        try
        {
            // … do heavy stuff …
            return $"pos={transform.position}, hp={HitPoints}, time={Time.time:F3}";
        }
        finally
        {
            if (Application.isPlaying) DestroyImmediate(temp);
        }
    }

    void SimulatePathfindingFailure()
    {
        // Throw to demonstrate error logging
        throw new System.InvalidOperationException("No path found to target within allotted budget.");
    }
}