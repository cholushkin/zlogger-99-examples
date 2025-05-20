using System.Collections.Generic;
using UnityEngine;

namespace Logging.Config
{
    [CreateAssetMenu(fileName = "LoggerConfiguration", menuName = "Logging/Logger Configuration")]
    public class LoggerConfiguration : ScriptableObject
    {
        [Tooltip("List of enabled log providers.")]
        public List<LoggerProviderConfigBase> Providers = new();
    }
}