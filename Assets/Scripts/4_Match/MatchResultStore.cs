using System.Net.Sockets;

public static class MatchResultStore
{
    public static int myPlayerNumber;
    public static string myNickname;
    public static string opponentNickname;
    public static string opponentIp;
    public static int opponentPort;
    public static int myPort;
    public static UdpClient udpClient;
}