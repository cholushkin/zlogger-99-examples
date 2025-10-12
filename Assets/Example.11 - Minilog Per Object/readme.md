# Minilog Per Object

ZLogger supports advanced logging strategies for Unity, including localized per-object logging. This sample shows how to attach a Minilog to individual GameObjects to visualize their logs directly in the editor's scene view.

## Features Demonstrated

- On-screen logging tied to individual GameObjects via a `Minilog` MonoBehaviour.
- Custom logger class with conditional routing to Minilog and the global logger.
- Configurable message storage with FIFO behavior and persistent message history.
- Dynamic rendering of log entries in the Unity Editor with level-based color coding.
- Deferred log message generation to avoid unnecessary computation.

## Summary

This example presents a flexible, per-object logging system in Unity using ZLogger. It’s ideal for debugging multiple objects independently in a scene by providing local, readable logs directly in the game view, helping developers isolate and inspect behaviors more effectively.
