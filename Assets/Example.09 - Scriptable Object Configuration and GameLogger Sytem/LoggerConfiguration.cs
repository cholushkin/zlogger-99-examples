using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace Logging.Config
{
    /*
    Intent of each knob
    =================================

    • Global HardFloor: a hard floor for everything.
      Nothing below this level should ever pass.
      Implement with a global AddFilter.

    • Global DefaultMin: a default minimum (soft) used if nothing
      more specific is set. Implement with SetMinimumLevel.

    • Per-provider HardFloor: a hard floor per provider
      (cannot be loosened by other filters).
      Implement with AddFilter<ThatProvider>.

    • Per-provider DefaultMin: a soft default per provider
      (can be tightened later). Implement with a second
      AddFilter<ThatProvider> only if you need to raise above global default.

    • Solo: if any provider has Solo = true, enable only those providers;
      ignore others (even if not muted).

    • Mute: skip configuring this provider entirely.


    Precedence (from strongest to weakest)
    ======================================

    1. Global hard floor (global AddFilter((cat, lvl) => lvl >= Global HardFloor))
    2. Provider hard floor (AddFilter<Provider>((cat, lvl) => lvl >= Provider HardFloor))
    3. Provider default min level (if > global DefaultMin)
    4. Global default min level (SetMinimumLevel(Global DefaultMin))
    5. Category/prefix filters you may add later (more specific wins)

    Rule of thumb:
    AddFilter is final for that scope.
    SetMinimumLevel is just a fallback.
    */

    [CreateAssetMenu(fileName = "LoggerConfiguration", menuName = "Logging/Logger Configuration")]
    public class LoggerConfiguration : ScriptableObject
    {
        [Serializable]
        public class ProviderConfiguration
        {
            public LoggerProviderConfigBase Provider;
            public LogLevel HardFloor;    // Can't override in Filters 
            public LogLevel DefaultMin;   // Can override in Filters
            public bool Solo;
            public bool Mute;
        }

        public LogLevel HardFloor;       // Can't override in Providers 
        public LogLevel DefaultMin;      // Can override in Providers
        [Tooltip("List of enabled log providers.")]
        public List<ProviderConfiguration> Providers = new();
    }
}
