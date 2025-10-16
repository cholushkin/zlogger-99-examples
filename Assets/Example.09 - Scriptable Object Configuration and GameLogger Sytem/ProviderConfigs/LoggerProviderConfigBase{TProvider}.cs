using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace Logging.Config
{
    /// <summary>
    /// Generic base implementing the common per-category Solo/Mute/MinLevel logic.
    /// Providers should inherit this (no reflection required).
    /// </summary>
    /// <typeparam name="TProvider">Concrete ILoggerProvider type used with AddFilter&lt;TProvider&gt;()</typeparam>
    public abstract class LoggerProviderConfigBase<TProvider> : LoggerProviderConfigBase
        where TProvider : ILoggerProvider
    {
        [Serializable]
        public class CategoryRule
        {
            [Tooltip("Category prefix (e.g., 'Game.', 'AI.'). Prefix match.")]
            public string CategoryPrefix = string.Empty;

            [Tooltip("Minimum level required for this prefix.")]
            public LogLevel MinLevel = LogLevel.Information;

            [Tooltip("If true, only Solo categories are kept active (others muted).")]
            public bool Solo;

            [Tooltip("If true, this category is completely muted.")]
            public bool Mute;
        }

        [Header("Provider-Scoped Category Filters")]
        public List<CategoryRule> CategoryFilters = new();

        // Provide concrete provider type for diagnostics
        public override Type ProviderType => typeof(TProvider);

        // Concrete subclasses must implement adding the provider itself.
        protected abstract void AddProvider(ILoggingBuilder builder, LoggerConfiguration root);

        /// <summary>
        /// Default provider-wide floor: adds a provider-scoped filter for all categories.
        /// Subclasses may override to use specialized AddFilter overloads if needed.
        /// </summary>
        protected virtual void ApplyProviderWideFloor(ILoggingBuilder builder, LogLevel minLevel)
        {
            builder.AddFilter<TProvider>(string.Empty, level => level >= minLevel);
        }

        /// <summary>
        /// Centralized Solo/Mute/MinLevel category filter logic — uses AddFilter&lt;TProvider&gt;.
        /// Subclasses can override to customize.
        /// </summary>
        protected virtual void ApplyProviderCategoryFilters(ILoggingBuilder builder, IReadOnlyList<CategoryRule> filters)
        {
            if (filters == null || filters.Count == 0) return;

            var rules = new List<CategoryRule>(filters);
            rules.RemoveAll(r => r == null);

            if (rules.Count == 0) return;

            // If any Solo exists, only keep those
            if (rules.Exists(r => r.Solo))
                rules.RemoveAll(r => !r.Solo);

            foreach (var r in rules)
            {
                if (string.IsNullOrEmpty(r.CategoryPrefix))
                {
                    // Empty prefix applies to all categories for this provider
                    if (r.Mute)
                    {
                        builder.AddFilter<TProvider>(string.Empty, _ => false);
                    }
                    else
                    {
                        builder.AddFilter<TProvider>(string.Empty, level => level >= r.MinLevel);
                    }
                }
                else
                {
                    if (r.Mute)
                    {
                        builder.AddFilter<TProvider>(r.CategoryPrefix, _ => false);
                    }
                    else
                    {
                        builder.AddFilter<TProvider>(r.CategoryPrefix, level => level >= r.MinLevel);
                    }
                }
            }
        }

        // Implementation of abstract base API
        public override void Configure(ILoggingBuilder builder, LoggerConfiguration root)
        {
            AddProvider(builder, root);
            ApplyProviderCategoryFilters(builder, CategoryFilters);
        }

        public override void SetProviderWideFloor(ILoggingBuilder builder, LogLevel minLevel)
        {
            if (minLevel < LogLevel.Trace) return;
            ApplyProviderWideFloor(builder, minLevel);
        }
    }
}
