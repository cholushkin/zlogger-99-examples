# ðŸªµ  [ZLogger](https://github.com/Cysharp/ZLogger) Integration Guide and sandbox for Unity
![project logo](doc-assets/repository-open-graph-cover.png)

## ðŸ“¦ Install ZLogger

- Use official instruction https://github.com/Cysharp/ZLogger/tree/master?tab=readme-ov-file#installation
- You can follow commit messages from version history to see the affect of each step.

## Examples

| Example | Description                                                   | Tags                  |
|---------|---------------------------------------------------------------|-----------------------|
| [**Example.00 - UnityLog**](Assets/Example.00%20-%20UnityLog) | Gives you some impression of what standard Unity logging provides. | `no-zlogger` `unity-standard`    |
| [**Example.01 - Basic Log Levels**](Assets/Example.01%20-%20Basic%20log%20levels) | Demonstrates ZLogger log levels and formatter configurations. | `#basic`, `#levels`    |
| [**Example.02 - Logging Scopes**](Assets/Example.02%20-%20Logging%20scopes) | Logging with named and structured scopes. | `#scopes`, `#structured` |
| [**Example.03 - File Provider**](Assets/Example.03%20-%20File%20provider) | Outputting logs to file via ZLogger file providers. | `#file`, `#provider`, `rolling` |
| [**Example.04 - Streams**](Assets/Example.04%20-%20Streams) | Outputting logs to memory stream. | `#stream`, `#provider` |
| [**Example.05 - InMemory**](Assets/Example.05%20-%20InMemory) | Using the InMemory provider. | `#inmemory`, `#provider` |
| [**Example.06 - Processor**](Assets/Example.06%20-%20Processor) | Using custom log processors. | `#processor`, `#provider` |
| [**Example.07 - PlainTextFormatter**](Assets/Example.07%20-%20PlainTextFormatter) |  Adding scope information when using the PlainTextFormatter. | `#plaintext`, `#formatter` |
| [**Example.08 - Per Provider Filtering**](Assets/Example.08%20-%20Per%20Provider%20Filtering) | Demonstrates how to apply different log filtering rules per provider. | `#filtering`, `#provider` |
| [**Example.09 - Scriptable Object Configuration and GameLogger System**](Assets/Example.09%20-%20Scriptable%20Object%20Configuration%20and%20GameLogger%20System) | Uses ScriptableObject for configuring log settings and managing GameLogger. | `#scriptableobject`, `#configuration`, `#gamelogger` |
| [**Example.10 - Expensive Interpolation Avoidance**](Assets/Example.10%20-%20Expensive%20Interpolation%20Avoidance) | Shows how to avoid unnecessary string interpolation for performance. | `#performance`, `#interpolation` |
| [**Example.11 - Minilog Per Object**](Assets/Example.11%20-%20Minilog%20Per%20Object) | Logging per object using a minimal logger setup. | `#minilog`, `#per-object` |

todo:
- Formatter configurations  
  - PlainText
  - JSON
  - KeyNameMutator
  - MessagePack
  - Custom Formatter
- json config / disable on build
+ scriptable object config
- LogInfo
  - Message Meta
- ZLoggerOptions
- ZLoggerMessage Source Generator
- Using with VContainer
- Implement some commong gamedev patterns
  + Global LoggerFactory
  - Unity decoraators
  + scriptable object configuration
  - uconsole
  + little logger for separate gameobject + widget
