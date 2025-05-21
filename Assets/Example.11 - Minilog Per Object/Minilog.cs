using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using UnityEditor;
using UnityEngine;

public class Minilog : MonoBehaviour
{
    [Tooltip("Maximum number of messages stored (FIFO)")]
    public int MaxLines = 8;

    public bool SaveAllMessages = true;

    public List<string> AllMessages = new List<string>();
    private readonly Queue<(int id, LogLevel level, string message)> _messages = new();
    private int _messageCounter = 1;

    public void AddMessage(LogLevel logLevel, string message)
    {
        if (_messages.Count >= MaxLines)
        {
            _messages.Dequeue(); // FIFO behavior
        }

        if (SaveAllMessages)
            AllMessages.Add(message);
        _messages.Enqueue((_messageCounter++, logLevel, message));
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !enabled)
            return;

#if UNITY_EDITOR
        Handles.BeginGUI();

        Vector3 worldPos = transform.position + Vector3.up * 2f;
        Vector3 screenPos = HandleUtility.WorldToGUIPoint(worldPos);

        GUIStyle headerStyle = new GUIStyle(EditorStyles.label);
        headerStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(screenPos.x, screenPos.y, 300, 20), $"[MINILOG for '{gameObject.name}']", headerStyle);

        float lineHeight = 18f;
        int line = 1;

        foreach (var (id, level, message) in _messages)
        {
            GUIStyle msgStyle = new GUIStyle(EditorStyles.label);
            msgStyle.normal.textColor = GetColorForLogLevel(level);
            string numberedMessage = $"#{id}: {message}";
            GUI.Label(new Rect(screenPos.x, screenPos.y + lineHeight * line, 600, 20), numberedMessage, msgStyle);
            line++;
        }

        Handles.EndGUI();
#endif
    }

    private Color GetColorForLogLevel(LogLevel level) =>
        level switch
        {
            LogLevel.Trace => new Color(0.7f, 0.7f, 0.7f),
            LogLevel.Debug => Color.gray,
            LogLevel.Information => Color.white,
            LogLevel.Warning => Color.yellow,
            LogLevel.Error => new Color(1f, 0.5f, 0.5f),
            LogLevel.Critical => Color.red,
            _ => Color.white
        };
}