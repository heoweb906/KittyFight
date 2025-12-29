using UnityEngine;

public static class AppLifecycle
{
    public static bool IsQuitting { get; private set; }
    public static bool IsDisconnecting { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reset()
    {
        IsQuitting = false;
        IsDisconnecting = false;

        Application.quitting += () =>
        {
            IsQuitting = true;
            IsDisconnecting = true;
        };
    }

    public static void BeginDisconnect()
    {
        IsDisconnecting = true;
    }
}