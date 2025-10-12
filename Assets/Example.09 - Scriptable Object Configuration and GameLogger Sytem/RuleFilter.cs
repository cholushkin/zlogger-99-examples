using Microsoft.Extensions.Logging;
using System;
using UnityEngine;

[Serializable]
public class RuleFilter
{
    [Tooltip("Logger provider type name (e.g. ZLoggerFileLoggerProvider). Leave empty for all providers.")]
    public string ProviderTypeName = "";

    [Tooltip("Category name. Leave empty for all categories.")]
    public string Category = "";

    [Tooltip("Minimum log level.")] public LogLevel MinLevel = LogLevel.Information;

    public bool AppliesToProvider(Type providerType)
    {
        return string.IsNullOrEmpty(ProviderTypeName)
               || providerType.Name == ProviderTypeName
               || providerType.FullName == ProviderTypeName;
    }
}