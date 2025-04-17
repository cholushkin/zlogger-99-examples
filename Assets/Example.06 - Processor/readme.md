# Log Processor Examples

This example suite showcases how to use custom log processors with the ZLogger library in Unity. Log processors in ZLogger allow developers to intercept, transform, and redirect logs to various targets, supporting asynchronous processing and flexible configuration.

## Features Demonstrated

- Creating a log processor (`UnityDebugLogProcessor`) to redirect logs to Unity's console.
- Leveraging `LoggerFactory` to plug in custom logging strategies via `AddZLoggerLogProcessor`.
- Implementing an in-game `MiniConsole` to collect and display logs interactively.
- Applying minimum log level filtering in custom log processors.
- Proper logger lifecycle management tied to Unity MonoBehaviour components.

## Summary

This code demonstrates how to integrate ZLogger’s log processing capabilities into Unity applications through modular components. By defining custom processors and registering them with the logging factory, developers gain fine-grained control over how logs are formatted, filtered, and displayed.
