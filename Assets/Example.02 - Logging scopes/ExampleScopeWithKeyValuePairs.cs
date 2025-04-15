using UnityEngine;
using ZLogger;
using Microsoft.Extensions.Logging;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using System.Collections.Generic;
using System.Linq;


/*
    📌 Why Use IEnumerable<KeyValuePair<string, object>>?
    ✅ Flexibility: You can dynamically yield items based on conditions, allowing lazy evaluation.
    ✅ Integration: Easily plug in data from systems that already produce KeyValuePair-based collections.
    ⚠️ Drawback: Slightly more verbose and less readable than anonymous objects or dictionaries for most use cases.
 */
public class ExampleScopeWithKeyValuePairs : MonoBehaviour
{
    [Header("Logging Controls")]
    public LogLevel LogLevel = LogLevel.Information;

    [Header("Player Info")]
    public int PlayerId = 42;
    public string PlayerName = "HeroCat";
    public string Area = "Forgotten Ruins";

    private ILogger _logger = null!;
    private ILoggerFactory _loggerFactory = null!;

    void Awake()
    {
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel);

            builder.AddZLoggerUnityDebug(options =>
            {
                options.IncludeScopes = true;
                options.UseJsonFormatter(); // Best visibility for structured state
            });
        });

        _logger = _loggerFactory.CreateLogger<ExampleScopeWithKeyValuePairs>();
    }

    void Start()
    {
        var scopeState = CreateScopeAsKeyValuePairs(PlayerId, PlayerName, Area);

        using (_logger.BeginScope(scopeState))
        {
            _logger.ZLogInformation($"Player entered a new zone.");
        }

        _logger.ZLogInformation($"Outside of custom key-value scope.");
    }

    /// Constructs a scope state using IEnumerable of KeyValuePair.
    private IEnumerable<KeyValuePair<string, object>> CreateScopeAsKeyValuePairs(int id, string name, string area)
    {
        yield return new KeyValuePair<string, object>("playerId", id);
        yield return new KeyValuePair<string, object>("playerName", name);
        yield return new KeyValuePair<string, object>("zone", area);
    }

    void OnDestroy()
    {
        _loggerFactory?.Dispose();
    }
}