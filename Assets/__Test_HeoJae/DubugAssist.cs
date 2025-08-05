using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DubugAssist : MonoBehaviour
{
    public TextMeshProUGUI debugTextUI;
    public int maxLines = 20;

    private Queue<string> logQueue = new();

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (logQueue.Count >= maxLines)
            logQueue.Dequeue();

        logQueue.Enqueue(logString);
        debugTextUI.text = string.Join("\n", logQueue);
    }
}
