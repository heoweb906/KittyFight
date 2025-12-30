public static class P2PMessageSender
{
    public static void SendMessage(string message)
    {
        if (AppLifecycle.IsDisconnecting) return;

        P2PManager.SendRaw(message); // P2PManager와 직접 연결
    }
}