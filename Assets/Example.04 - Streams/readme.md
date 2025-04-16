# Stream Provider Examples

These examples illustrate how to utilize stream providers with the ZLogger library for logging in Unity. Stream providers allow ZLogger to direct log output to various streams.

## Features Demonstrated:

* **In-Memory Logging:** The `ExampleMemoryStream` script demonstrates logging to a `MemoryStream`.
    * ZLogger is configured to write to MemoryStream.
    * The script shows how to read the logged data from the `MemoryStream`.
* **Compressed File Logging:** The `ExampleZipStream` script demonstrates logging to a compressed file using `GZipStream`.
    * It creates a `FileStream` and wraps it with a `GZipStream` for compression.
    * ZLogger is set up to write to the `GZipStream`.
* **ZLogger Configuration:**
    * Both examples use `LoggerFactory.Create` to configure ZLogger.
    * `AddZLoggerStream` is used to specify the output stream.
    * `UsePlainTextFormatter` is used to customize log formatting.

## Summary

These examples illustrate the practical application of ZLogger's stream provider functionality. They demonstrate how to log to memory and compressed files, showcasing key ZLogger configuration techniques for stream output, formatting, and log level management.