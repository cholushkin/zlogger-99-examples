using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using UnityEngine;

public abstract class LoggerProviderConfigBase : ScriptableObject
{
    public bool Enable = true;

    [Tooltip("Filtering rules for this provider.")]
    public List<RuleFilter> Filters = new();

    public abstract void Configure(ILoggingBuilder builder, string logDirectory);

    protected void ApplyFilters<TProvider>(ILoggingBuilder builder) where TProvider : ILoggerProvider
    {
        foreach (var rule in Filters)
        {
            if (rule.AppliesToProvider(typeof(TProvider)))
            {
                builder.AddFilter<TProvider>(rule.Category, rule.MinLevel);
            }
        }
    }
}