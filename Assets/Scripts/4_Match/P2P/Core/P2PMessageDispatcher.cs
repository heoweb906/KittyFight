using System.Collections.Generic;

public static class P2PMessageDispatcher
{
    private static List<IP2PMessageHandler> handlers = new();

    public static void RegisterHandler(IP2PMessageHandler handler)
    {
        if (!handlers.Contains(handler))
            handlers.Add(handler);
    }

    public static void UnregisterHandler(IP2PMessageHandler handler)
    {
        handlers.Remove(handler);
    }

    public static void Dispatch(string msg)
    {
        foreach (var handler in handlers)
        {
            if (handler.CanHandle(msg))
            {
                handler.Handle(msg);
                return;
            }
        }

        UnityEngine.Debug.LogWarning($"Unhandled P2P message: {msg}");
    }
    public static void ClearAllHandlers()
    {
        handlers.Clear();
    }
}