// File: GameLifetimeScope.cs
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Logging.Config;
using Game.Registration;

namespace Game.Composition
{
    /// <summary>
    /// Root lifetime scope for the entire game.
    /// Lives across scenes and registers all global services in one place.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-10000)]  // very early
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [Header("Logging")]
        [SerializeField] private LoggerConfiguration loggingConfig;
        [SerializeField] private bool loggingEnabled = true;

        [Header("Lifetime")]
        [Tooltip("If true, this scope persists across scene loads and acts as the app root.")]
        [SerializeField] private bool dontDestroyOnLoad = true;

        private static GameLifetimeScope _instance;

        protected override void Awake()
        {
            // Prevent duplicates if this prefab/GO appears twice
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning("[GameLifetimeScope] Duplicate root detected, destroying this one.");
                Destroy(gameObject);
                return;
            }
            _instance = this;

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            base.Awake(); // builds the container & runs startables
        }

        protected override void Configure(IContainerBuilder builder)
        {
            // Register global services here
            builder.RegisterLogging(loggingConfig, loggingEnabled);

            // Examples for later:
            // builder.RegisterSaveSystem(saveConfig);
            // builder.RegisterAnalytics(analyticsConfig);
            // builder.RegisterLocalization(localizationConfig);
        }
    }
}
