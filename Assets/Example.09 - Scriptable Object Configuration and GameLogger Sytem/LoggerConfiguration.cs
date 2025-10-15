using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using NaughtyAttributes;

namespace Logging.Config
{
    /*
    Intent of each knob
    =================================
    • Global HardFloor — absolute clamp for all outputs (global AddFilter).
    • Global DefaultMin — soft default when nothing more specific applies (SetMinimumLevel).
    • Per-provider HardFloor — clamp for that provider (AddFilter<ThatProvider>).
    • Per-provider DefaultMin — effective only if STRICTER than Global DefaultMin (otherwise ignored).
    • Solo — if any provider has Solo = true, only Solo providers are enabled.
    • Mute — provider is skipped entirely.

    Precedence (strongest → weakest)
    1) Global HardFloor
    2) Provider HardFloor
    3) Provider DefaultMin (only if > Global DefaultMin)
    4) Global DefaultMin
    5) Category/prefix filters (can only tighten for their prefixes)

    Rule of thumb:
    AddFilter is final for that scope. SetMinimumLevel is a fallback.
    */

    [CreateAssetMenu(fileName = "LoggerConfiguration", menuName = "Logging/Logger Configuration")]
    public class LoggerConfiguration : ScriptableObject
    {
        [Serializable]
        public class ProviderConfiguration
        {
            [BoxGroup("Provider"), Required]
            public LoggerProviderConfigBase Provider;

            [BoxGroup("Provider")]
            [Label("Hard Floor")]
            [Tooltip("Provider-wide hard floor (cannot be loosened by other filters).")]
            public LogLevel HardFloor;

            [BoxGroup("Provider/Advanced")]
            [Label("Default Min")]
            [Tooltip("Soft default for this provider. Effective only if stricter than Global DefaultMin.")]
            public LogLevel DefaultMin;

            [BoxGroup("Provider/Flags")]
            public bool Solo;

            [BoxGroup("Provider/Flags")]
            public bool Mute;
        }

        // Single, concise guide at the top
        [InfoBox(
            "•Global HardFloor is an ABSOLUTE clamp; nothing below it passes.\n" +
            "•Global DefaultMin is a soft default used when nothing more specific applies.\n" +
            "•Provider HardFloor clamps ALL categories for that provider.\n" +
            "•Provider DefaultMin takes effect ONLY if it is STRICTER than Global DefaultMin.\n" +
            "•Category filters (by prefix) can only TIGHTEN further for those prefixes.\n" +
            "•Solo: if any provider has Solo=true, only Solo providers are enabled.\n" +
            "\n" +
            "Tips:\n"+ 
            "•Keep DefaultMin ≥ HardFloor (global & per-provider).\n" +
            "•Use Provider DefaultMin sparingly; prefer category filters.\n" +
            "•In production, raise Global HardFloor (Warning/Error) and keep file provider stricter than console.\n" +
            "•Apply provider-wide floors BEFORE category rules (done in LogManager).\n"
            )]
        [Label("Global Hard Floor")]
        public LogLevel HardFloor;

        [Label("Global Default Min")]
        public LogLevel DefaultMin;

        [Tooltip("Enabled log providers and their local rules.")]
        public List<ProviderConfiguration> Providers = new();

#if UNITY_EDITOR
        void OnValidate()
        {
            // Global rule: DefaultMin must be >= HardFloor
            if (DefaultMin < HardFloor)
            {
                Debug.LogWarning(
                    $"[LoggerConfiguration] Global DefaultMin ({DefaultMin}) is lower than Global HardFloor ({HardFloor}). " +
                    $"Raising DefaultMin to {HardFloor}.");
                DefaultMin = HardFloor;
                EditorUtility.SetDirty(this);
            }

            // Provider rule: Provider.DefaultMin must be >= Provider.HardFloor
            foreach (var p in Providers)
            {
                if (p == null) continue;

                if (p.DefaultMin < p.HardFloor)
                {
                    Debug.LogWarning(
                        $"[LoggerConfiguration] Provider '{p.Provider?.name ?? "<null>"}': DefaultMin ({p.DefaultMin}) " +
                        $"is lower than HardFloor ({p.HardFloor}). Raising DefaultMin to {p.HardFloor}.");
                    p.DefaultMin = p.HardFloor;
                    EditorUtility.SetDirty(this);
                }
            }
        }
#endif
    }
}
