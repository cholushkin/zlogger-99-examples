# Scriptable Object Configuration and GameLogger Sytem

This example demonstrates how to build a flexible logging system using ZLogger in Unity by combining ScriptableObject-based configuration with a lightweight game-side `Logger` abstraction. The setup allows you to define and control logging behavior in the Unity Editor without code changes.

## Features Demonstrated

- Use of `ScriptableObject` to configure global log level and active logging providers
- Custom `Logger` class with local logging level and enable flag
- Lazy initialization of loggers through a singleton `LogManager`
- Support for different logging providers (file and Unity console)
- Provider-specific filter rules using `RuleFilter` configuration
- Integration of logger instances in game components (e.g., `Monster`, `SuperHero`)

## Summary

This example showcases a modular and configurable logging architecture using ScriptableObjects and ZLogger. It enables runtime log control, provider-specific filtering, and component-level logging without hardcoding logger setup. The structure encourages clean separation between logging configuration and game logic.
