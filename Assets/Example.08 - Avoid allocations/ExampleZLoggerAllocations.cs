using UnityEngine;
using Microsoft.Extensions.Logging;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using System;

public class ExampleZLoggerAllocations : MonoBehaviour
{
    private ILogger _logger = null!;
    private ILoggerFactory _factory = null!;
    public int Iterations = 100_000;
    
    
    // [Improper] 1000000 logs: ΔMem = 102,789,120 bytes, Time = 2799.2 ms
    // [Proper] 1000000 logs: ΔMem = 1,884,160 bytes, Time = 2523.0 ms



    void Awake()
    {
        // 1. Create the logger factory
        _factory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace);

            // 2. Use InMemory sink, but ignore messages
            builder.AddZLoggerStream(System.IO.Stream.Null, options =>
            {
                options.UsePlainTextFormatter();
            });

        });

        // 3. Create logger instance
        _logger = _factory.CreateLogger("ZeroAllocTest");
    }

    void Start()
    {
        int id = 42;
        string name = "Meo";

        // Warm-up to JIT everything
        _logger.ZLogInformation($"Warmup {name}");

        // Improper logging: prebuilt string (allocates)
        MeasureAlloc("Improper", () =>
        {
            for (int i = 0; i < Iterations; i++)
            {
                string msg = $"Improper log: Player {name} with ID {id}";
                _logger.Log(LogLevel.Information, msg); // prebuilt string
            }
        });

        // Proper logging: inline interpolation (zero alloc)
        MeasureAlloc("Proper", () =>
        {
            for (int i = 0; i < Iterations; i++)
            {
                _logger.ZLogInformation($"Proper log: Player {name} with ID {id}");
            }
        });
    }

    void MeasureAlloc(string label, Action action)
    {
        // Force GC before measurement
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long before = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
        var startTime = Time.realtimeSinceStartup;

        action();

        float elapsed = Time.realtimeSinceStartup - startTime;
        long after = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
        long delta = after - before;

        Debug.LogWarning($"[{label}] {Iterations} logs: ΔMem = {delta:N0} bytes, Time = {elapsed * 1000f:F1} ms");
    }

    void OnDestroy() => _factory.Dispose();
}