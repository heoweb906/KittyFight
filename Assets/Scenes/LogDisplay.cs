using UnityEngine;
using TMPro;

public class LogDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text debugText;

    void Awake()
    {
        debugText.text = "";
    }

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
        if (type == LogType.Warning) return;

        // 새 로그를 앞에 붙이고, 그 뒤에 기존 텍스트를 붙임
        debugText.text = logString + "\n" + debugText.text;
    }
}