public static class P2PMessageSender
{
    public static void SendMessage(string message)
    {
        if (AppLifecycle.IsDisconnecting) return;

        if (MatchResultStore.useSteam)
            SteamP2PManager.SendRaw(message);
        else
            P2PManager.SendRaw(message);
    }
}