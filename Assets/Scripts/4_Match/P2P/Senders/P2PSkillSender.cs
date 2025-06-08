public static class P2PSkillSender
{
    public static void SendMessage(string message)
    {
        P2PManager.SendRaw(message); // P2PManager와 직접 연결
    }
}
