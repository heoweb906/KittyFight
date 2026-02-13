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

    public static bool useSteam;
    public static string mySteamId;
    public static string opponentSteamId;
    public static string myNatType;
    public static string opponentNatType;
    public static bool opponentRelayMarker;

    public static void Reset()
    {
        try
        {
            if (udpClient != null)
            {
                udpClient.Close();
                udpClient.Dispose();
            }
        }
        catch
        {
        }
        finally
        {
            udpClient = null;
        }

        myPlayerNumber = 0;
        myNickname = null;
        opponentNickname = null;
        opponentIp = null;
        opponentPort = 0;
        myPort = 0;

        useSteam = false;
        mySteamId = null;
        opponentSteamId = null;
        myNatType = null;
        opponentNatType = null;
        opponentRelayMarker = false;
    }
}