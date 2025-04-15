# File Provider Examples

These examples illustrate how ZLogger's file provider can be used to manage log output to files in Unity. ZLogger's file provider component offers various options for configuring file-based logging within the ZLogger library.

## Features Demonstrated

* **Daily Rolling and Size-Based Rolling:** The `ExampleDailyRollingJson.cs` script demonstrates how to create log files that roll over daily using a date-based filename template. Additionally, it showcases how to implement file size limits, adding an index to the filename when the maximum size is reached to create new files within the same day.
* **Basic File Output:** The `ExampleFileProviderBasic.cs` script provides a fundamental example of writing log messages to a file.
* **File Write Mode Selection:** The `ExampleFileProviderBasic.cs` script demonstrates the option to choose between appending log messages to an existing file or rewriting/overwriting the file on each run.
* **Custom Log Prefix Formatting:** The `ExampleFileProviderBasic.cs` script exemplifies customizing log message prefixes, specifically by adding a timestamp to each log entry.

## Summary

These examples provide a collection of approaches for configuring file output using ZLogger's file provider features. They demonstrate techniques for managing log file rotation, selecting different file writing modes, and customizing log message formatting. The examples cover a range of scenarios including daily rolling, basic file writing, and controlling the format and structure of log output.