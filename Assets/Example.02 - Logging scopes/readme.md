# ZLogger Scope Examples for Unity

This collection demonstrates various ways to use ZLogger's `BeginScope` functionality within Unity projects to add contextual information to your logs.

## Features Demonstrated

-   **Integration with Unity Console:** Shows how to configure ZLogger to output logs to the Unity Debug Console using `AddZLoggerUnityDebug`.
-   **Multiple Formatters:** Examples utilize both `UsePlainTextFormatter` and `UseJsonFormatter` to show different output styles, with JSON being particularly useful for structured scope data.
-   **Named Scopes:** Usage of simple `string` values in `BeginScope` (like `"InitializationScope"` or `"order-processing"`) to logically group related log entries (`ExampleStringScope`, `ExampleBasicScopeStateAnonymousObject`).
-   **Structured Scopes with State:** Demonstrates adding key-value data to scopes, which persists for all logs within that scope (and nested scopes):
  -   Using **anonymous objects** (e.g., `new { PlayerId = 99, Level = "QuantumCatNebula" }`) for concise, fixed state (`ExampleBasicScopeStateAnonymousObject`).
  -   Using **custom class instances** (e.g., passing a `Player` object) to represent scope context (`ExampleScopeWithCustomObject`).
  -   Using **`Dictionary<string, object>`** for dynamically building scope state based on conditions (`ExampleScopeWithDynamicDictionary`).
  -   Using **interpolated strings** (e.g., `$"UserSession: userId={userId}"`) for dynamic descriptive scope names (`ExampleScopeWithInterpolatedString`). *Note: Primarily provides a descriptive label rather than structured key-value pairs like other methods unless specifically parsed by a downstream system.*
  -   Using **`IEnumerable<KeyValuePair<string, object>>`** for flexible or lazily evaluated scope state (`ExampleScopeWithKeyValuePairs`).
  -   Using **message templates** (e.g., `"User {UserId} performed action {ActionType}"`, userId, action) where the template defines both a readable message and structured keys (`ExampleScopeWithTemplate`).
-   **Nested Scopes:** Shows how scopes can be nested (e.g., a "query" scope inside an "order-processing" scope), inheriting context from parent scopes.
-   **Log Level Configuration:** Setting the minimum log level via `SetMinimumLevel` and demonstrating how scopes still execute their internal logic even if the log level prevents messages from being output.
-   **Resource Management:** Clean disposal of the `ILoggerFactory` instance within the `OnDestroy` method to prevent resource leaks.

## Summary

These examples provide practical illustrations of how to leverage ZLogger scopes in Unity. They cover basic setup, simple named scopes for organization, and various powerful techniques for adding structured contextual data (like Player ID, Session Info, Action Type) to logs using anonymous objects, custom classes, dictionaries, KeyValuePairs, and message templates. This structured context is invaluable for debugging and analyzing logs, especially when using formatters like JSON. Proper logger factory lifecycle management is also emphasized.