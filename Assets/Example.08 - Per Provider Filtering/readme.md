# Per Provider Filtering

ZLogger's Per Provider Filtering feature enables the definition of individual log level filters for each logging provider. This helps manage verbosity and tailor log outputs based on destination.

## Features Demonstrated

- Initialization of `ILoggerFactory` with multiple providers
- Unity console provider logs all levels (Trace and above)
- File provider logs only `Error` level and above
- Demonstration of `AddFilter<TProvider>` to apply per-provider filtering
- Logging of various message levels to observe provider-specific filtering
- File logging to Unity’s persistent data path

## Summary

This code demonstrates how to apply separate filtering rules to different logging providers using ZLogger. It illustrates practical logging control, showing how certain levels appear only in the Unity console, while critical messages are also persisted to a file.
