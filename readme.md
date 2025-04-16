# 🪵  ZLogger Integration Guide and sandbox for Unity
![project logo](doc-assets/repository-open-graph-cover.png)

## 📦 Install ZLogger

- Use official instruction https://github.com/Cysharp/ZLogger/tree/master?tab=readme-ov-file#installation
- You can follow commit messages from version history to see the affect of each step.

## Examples

| Example | Description                                                   | Tags                  |
|---------|---------------------------------------------------------------|-----------------------|
| [**Example.00 – UnityLog**](Assets/Example.00%20-%20UnityLog) | Gives you some impression of what standard Unity logging provides. | `no-zlogger` `unity-standard`    |
| [**Example.01 – Basic Log Levels**](Assets/Example.01%20-%20Basic%20log%20levels) | Demonstrates ZLogger log levels and formatter configurations. | `#basic`, `#levels`    |
| [**Example.02 – Logging Scopes**](Assets/Example.02%20-%20Logging%20scopes) | Logging with named and structured scopes. | `#scopes`, `#structured` |
| [**Example.03 – File Provider**](Assets/Example.03%20-%20File%20provider) | Outputting logs to file via ZLogger file providers. | `#file`, `#provider`, `rolling` |


todo:
- Logging Providers
  - Console
  + File
  + RollingFile
  - Stream 
  - In-Memory
  - LogProcessor
- Formatter configurations  
  - PlainText
  - JSON
  - KeyNameMutator
  - MessagePack
  - Custom Formatter
- json config / disable on build
- scriptable object config
- LogInfo
  - Message Meta
- ZLoggerOptions
- ZLoggerMessage Source Generator
- Using with VContainer
- Implement some commong gamedev patterns
  - Global LoggerFactory
  - Unity decoraators
  - json configuration
  - uconsole
  - little logger for separate gameobject + widget
