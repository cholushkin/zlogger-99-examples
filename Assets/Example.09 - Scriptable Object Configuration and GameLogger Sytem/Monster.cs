using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger;

public class Monster : MonoBehaviour
{
    public Example09.Logger Logger;

    void Start()
    {
        // ❌ We can’t use this simpler form:
        // Logger.Log(LogLevel.Information, $"Initializing Monster");
        //
        // Because Unity currently runs on a .NET Standard 2.1 / .NET Framework–based runtime
        // (not full .NET 6 or higher). ZLogger’s zero-allocation logging relies on
        // the C# 10 feature “interpolated string handlers” together with the
        // [InterpolatedStringHandlerArgument] attribute for compile-time code generation.
        // That attribute and its compiler behavior are only available in .NET 6+.
        //
        // Without .NET 6, implementing a proper wrapper that preserves
        // ZLogger’s zero-alloc behavior isn’t possible — the compiler won’t
        // recognize the attribute and will treat the interpolated string as a normal one,
        // causing allocations. Therefore, we call ZLogger directly like this instead:
        Logger.Instance().ZLog( Logger.Level(LogLevel.Information), $"Initializing Monster");
    }
}