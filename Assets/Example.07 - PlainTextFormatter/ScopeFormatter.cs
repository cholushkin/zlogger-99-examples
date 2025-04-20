using System.Text;
using ZLogger;

public static class ScopeFormatter
{
    //⚠️ Only use a shared StringBuilder if you guarantee single-threaded access (like in Unity's main thread).
    // Otherwise, prefer local instances or thread-local storage.
    static readonly StringBuilder _sharedStringBuilder = new StringBuilder(128);

    public static string FormatScope(LogScopeState? scopeState)
    {
        if (scopeState == null || scopeState.IsEmpty)
            return string.Empty;

        var sb = _sharedStringBuilder;
        sb.Clear();

        sb.Append('[');
        var first = true;
        foreach (var kvp in scopeState.Properties)
        {
            if (!first)
                sb.Append(", ");

            sb.Append(kvp.Key).Append('=').Append(kvp.Value?.ToString() ?? "null");
            first = false;
        }
        sb.Append(']');
        return sb.ToString();
    }
}