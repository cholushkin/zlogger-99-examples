// File: RegistrationExtensions.cs
using System;
using Microsoft.Extensions.Logging;
using VContainer;
using VContainer.Unity;
using Logging.Config;
using Logging.Runtime;

namespace Game.Registration
{
    /// <summary>
    /// Registration helpers for global/feature modules.
    /// Call these from your root LifetimeScope.
    /// </summary>
    public static class VContainerRegistrationExtensions
    {
        public static void RegisterLogging(this IContainerBuilder builder, LoggerConfiguration config, bool enabled)
        {
            // Service (singleton) + lifecycle hooks
            builder.Register(_ => new LogManagerService(config, enabled), Lifetime.Singleton)
                .As<ILogManagerService>()
                .As<IStartable>()
                .As<IDisposable>();

            // Also expose ILoggerFactory directly for convenience
            builder.Register(r => r.Resolve<ILogManagerService>().Factory, Lifetime.Singleton)
                .As<ILoggerFactory>();
            
            //builder.RegisterComponentInHierarchy<ResolveLogManagerServiceExample>();
        }

        // Example stubs for future services:
        // public static void RegisterSaveSystem(this IContainerBuilder b, SaveConfig cfg) { ... }
        // public static void RegisterAnalytics(this IContainerBuilder b, AnalyticsConfig cfg) { ... }
        // public static void RegisterLocalization(this IContainerBuilder b, LocalizationConfig cfg) { ... }
    }
}