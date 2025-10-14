using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace Logging.Config
{
    /// Base class for logging provider configurations.
    /// Concrete providers must:
    ///  - Register themselves in AddProvider()
    ///  - Apply provider-scoped category filters in ApplyProviderCategoryFilters()
    ///  - Apply provider-wide minimum level in ApplyProviderWideFloor()
    public abstract class LoggerProviderConfigBase : ScriptableObject
    {
        [Serializable]
        public class CategoryRule
        {
            [Tooltip("Category name or prefix (e.g., \"MyGameLogger\" or \"GameLogic.\"). Prefix match.")]
            public string CategoryPrefix = "";

            [Tooltip("Minimum level for categories that start with this prefix (provider-specific).")]
            public LogLevel MinLevel = LogLevel.Information;
        }

        [Header("Provider-scoped category filters")]
        [Tooltip("Rules apply only to this provider. CategoryPrefix is matched by prefix.")]
        public List<CategoryRule> CategoryFilters = new();

        // Concrete provider type (for diagnostics/UI only).
        public abstract Type ProviderType { get; }

        // Registers the provider with the logging builder. No filters here.
        protected abstract void AddProvider(ILoggingBuilder builder, LoggerConfiguration root);

        // Applies provider-specific category filters using the real provider type.
        // Called automatically by Configure() after AddProvider().
        protected abstract void ApplyProviderCategoryFilters(
            ILoggingBuilder builder,
            IReadOnlyList<CategoryRule> categoryFilters
        );

        // Applies a provider-wide minimum level (all categories) using the concrete provider type.
        protected abstract void ApplyProviderWideFloor(ILoggingBuilder builder, LogLevel minLevel);

        // Template: add provider, then apply its category filters.
        public virtual void Configure(ILoggingBuilder builder, LoggerConfiguration root)
        {
            AddProvider(builder, root);

            if (CategoryFilters != null && CategoryFilters.Count > 0)
            {
                ApplyProviderCategoryFilters(builder, CategoryFilters);
            }
        }

        // Called by LogManager to set a provider-wide floor without knowing the concrete type.
        public void SetProviderWideFloor(ILoggingBuilder builder, LogLevel minLevel)
        {
            if (minLevel < LogLevel.Trace) return;
            ApplyProviderWideFloor(builder, minLevel);
        }
    }
}
