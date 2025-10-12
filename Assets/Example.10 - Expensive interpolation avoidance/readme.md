# Expensive Interpolation Avoidance

ZLogger provides a powerful way to avoid unnecessary computation in logging by deferring expensive string interpolation until it's certain that a log message will be written. This sample shows how to apply that technique in a Unity environment using custom extension methods.

## Features Demonstrated

- Deferred logging using `Func<string>` to avoid expensive calculations.
- Logger extensions for multiple log levels (Info, Warning, Error, etc.).
- Real example of an expensive calculation being skipped when log level is not enabled.
- Setup of a logger with ZLogger in Unity, including pretty stack trace formatting.

## Summary

This example illustrates how to improve performance in logging-heavy applications by leveraging deferred evaluation. By using ZLogger’s ability to delay string interpolation, developers can skip expensive operations unless the message is actually going to be logged, resulting in cleaner and more efficient code.
