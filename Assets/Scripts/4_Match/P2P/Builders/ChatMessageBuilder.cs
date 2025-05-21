public static class ChatMessageBuilder
{
    public static string Build(string content)
    {
        return $"[CHAT]{content}";
    }
}