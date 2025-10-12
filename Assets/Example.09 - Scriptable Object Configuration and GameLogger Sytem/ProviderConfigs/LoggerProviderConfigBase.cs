using System.Collections.Generic;
using Logging.Config;
using Microsoft.Extensions.Logging;
using UnityEngine;

public abstract class LoggerProviderConfigBase : ScriptableObject
{
    public bool Enabled = true;
    public bool RespectGlobalLevelInFilter; // or completely override

    [Tooltip("Filtering rules for this provider.")]
    public List<RuleFilter> Filters = new();
    
    protected LoggerConfiguration _config = null;

    public virtual void Configure(ILoggingBuilder builder, LoggerConfiguration cfg)
    {
        _config = cfg;
    }

    protected void ApplyFilters<TProvider>(ILoggingBuilder builder, LoggerConfiguration cfg) where TProvider : ILoggerProvider
    {
        foreach (var rule in Filters)
        {
            if (rule.AppliesToProvider(typeof(TProvider)))
            {
                Debug.Log($"Adding filter to provider `{typeof(TProvider).FullName}`");
                
                builder.AddFilter<TProvider>(
                    rule.Category, 
                    RespectGlobalLevelInFilter ? (LogLevel)Mathf.Max((int)rule.MinLevel, (int)cfg.GlobalLogLevel) : rule.MinLevel);
            }
            else
            {
                Debug.Log($"this filter doesn't applicable to provider `{typeof(TProvider).FullName}`");
            }
        }
    }
}