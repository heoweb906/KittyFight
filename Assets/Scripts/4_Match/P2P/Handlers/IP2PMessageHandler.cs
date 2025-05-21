public interface IP2PMessageHandler
{
    bool CanHandle(string msg);
    void Handle(string msg);
}