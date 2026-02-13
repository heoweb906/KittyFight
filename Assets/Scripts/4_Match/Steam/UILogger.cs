using System;
using UnityEngine;
using TMPro;

public class UILogger : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text logText;
    public int maxChars = 20000;

    public void AppendLog(string msg)
    {
        if (logText == null)
        {
            Debug.Log(msg);
            return;
        }

        string line = $"[{DateTime.Now:HH:mm:ss.fff}] {msg}";
        logText.text += line + "\n";

        if (logText.text.Length > maxChars)
            logText.text = logText.text.Substring(logText.text.Length - maxChars);
    }

    public void Clear()
    {
        if (logText != null) logText.text = string.Empty;
    }
}