using UnityEngine;
using System.Collections.Generic;

public class CheckingDebug : MonoBehaviour
{
    private bool showConsole = false;
    private List<string> logs = new List<string>();
    private Vector2 scrollPosition;
    private const int maxLogs = 100;

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logEntry = $"[{type}] {logString}";

        logs.Add(logEntry);

        // �αװ� �ʹ� ������ ������ �ͺ��� ����
        if (logs.Count > maxLogs)
        {
            logs.RemoveAt(0);
        }
    }

    private void Update()
    {
        // 3�� �հ��� ��ġ�� �ܼ� ��� (�����)
        if (Input.touchCount == 3)
        {
            showConsole = !showConsole;
        }

        // Ű����ε� ��� ����
        if (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.F12))
        {
            showConsole = !showConsole;
        }
    }

    private void OnGUI()
    {
        if (!showConsole) return;

        float width = Screen.width * 0.8f;
        float height = Screen.height * 0.6f;
        float x = (Screen.width - width) * 0.5f;
        float y = (Screen.height - height) * 0.5f;

        GUI.Box(new Rect(x, y, width, height), "Debug Console");

        // ��ũ�� ����
        scrollPosition = GUI.BeginScrollView(
            new Rect(x + 10, y + 30, width - 20, height - 80),
            scrollPosition,
            new Rect(0, 0, width - 40, logs.Count * 20)
        );

        for (int i = 0; i < logs.Count; i++)
        {
            GUI.Label(new Rect(0, i * 20, width - 40, 20), logs[i]);
        }

        GUI.EndScrollView();

        // ��ư��
        if (GUI.Button(new Rect(x + 10, y + height - 40, 100, 30), "Clear"))
        {
            logs.Clear();
        }

        if (GUI.Button(new Rect(x + width - 110, y + height - 40, 100, 30), "Close"))
        {
            showConsole = false;
        }
    }
}