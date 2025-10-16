using System;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace Logging.Config
{
    /// <summary>
    /// Non-generic base so ScriptableObject references (in LoggerConfiguration) work.
    /// Concrete providers should generally inherit LoggerProviderConfigBase<TProvider>.
    /// </summary>
    public abstract class LoggerProviderConfigBase : ScriptableObject
    {
        /// <summary>
        /// For diagnostics / editor only. Implemented by generic subclass.
        /// </summary>
        public abstract Type ProviderType { get; }

        /// <summary>
        /// Called by LogManager to register provider and apply category filters.
        /// Concrete subclasses must implement.
        /// </summary>
        public abstract void Configure(ILoggingBuilder builder, LoggerConfiguration root);

        /// <summary>
        /// Called by LogManager to set a provider-wide floor.
        /// </summary>
        public abstract void SetProviderWideFloor(ILoggingBuilder builder, LogLevel minLevel);
    }
}