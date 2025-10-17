using System;
using Logging.Runtime;
using UnityEngine;
using VContainer;
using ZLogger;

public class ResolveLogManagerServiceExample : MonoBehaviour
{
    [Inject] private ILogManagerService Service;

    private void Start()
    {
        var logger = Service.CreateLogger("test-category");
        logger.ZLogInformation($"hi");

    }
}
