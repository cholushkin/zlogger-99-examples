using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace Logging.Config
{
    [CreateAssetMenu(fileName = "LoggerConfiguration", menuName = "Logging/Logger Configuration")]
    public class LoggerConfiguration : ScriptableObject
    {
        public LogLevel GlobalLogLevel;
        [Tooltip("List of enabled log providers.")]
        public List<LoggerProviderConfigBase> Providers = new();
    }
}