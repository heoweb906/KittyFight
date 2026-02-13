using Steamworks;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Text;

public class SteamP2PClient : MonoBehaviour
{
    [Header("Target Host")]
    [Tooltip("Host SteamID (ulong). Paste the host's SteamUser.GetSteamID() value here.")]
    public ulong hostSteamId;

    [Header("P2P Settings")]
    public int virtualPort = 0;

    [Header("Send Test")]
    public KeyCode sendKey = KeyCode.Space;
    public bool reliable = true;

    private HSteamNetConnection _conn = HSteamNetConnection.Invalid;

    void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("[CLIENT] SteamManager not initialized. SteamAPI.Init() must succeed first.");
            enabled = false;
            return;
        }

        SteamNetworkingUtils.InitRelayNetworkAccess();

        if (hostSteamId == 0)
        {
            Debug.LogError("[CLIENT] hostSteamId is 0. Set it in Inspector.");
            enabled = false;
            return;
        }

        var identity = new SteamNetworkingIdentity();
        identity.SetSteamID(new CSteamID(hostSteamId));

        _conn = SteamNetworkingSockets.ConnectP2P(ref identity, virtualPort, 0, null);
        Debug.Log($"[CLIENT] ConnectP2P => conn={_conn}, hostSteamId={hostSteamId}, mySteamId={SteamUser.GetSteamID()}");
    }

    void Update()
    {
        if (_conn == HSteamNetConnection.Invalid)
            return;

        if (Input.GetKeyDown(sendKey))
        {
            string msg = $"Hello from client @ {DateTime.Now:HH:mm:ss.fff}";
            bool ok = SendString(_conn, msg, reliable);
            Debug.Log(ok ? $"[CLIENT] Sent: {msg}" : "[CLIENT] Send failed.");
        }

        ReceiveLoop(_conn);
    }

    private static bool SendString(HSteamNetConnection conn, string s, bool reliable)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(s);

        // Steamworks.NET 버전에 따라 enum(ESteamNetworkingSend)이 없고 Constants 상수로 제공되는 경우가 많음
        int sendFlags = reliable
            ? Constants.k_nSteamNetworkingSend_Reliable
            : Constants.k_nSteamNetworkingSend_Unreliable;

        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            EResult r = SteamNetworkingSockets.SendMessageToConnection(
                conn,
                handle.AddrOfPinnedObject(),
                (uint)bytes.Length,
                sendFlags,
                out long messageNumber);

            if (r != EResult.k_EResultOK)
            {
                Debug.LogError($"[CLIENT] SendMessageToConnection failed: {r}");
                return false;
            }

            // 필요하면 messageNumber로 트래킹 가능
            return true;
        }
        finally
        {
            handle.Free();
        }
    }

    private static void ReceiveLoop(HSteamNetConnection conn)
    {
        // 한 프레임에 최대 16개까지 수신
        IntPtr[] ptrs = new IntPtr[16];
        int n = SteamNetworkingSockets.ReceiveMessagesOnConnection(conn, ptrs, ptrs.Length);

        for (int i = 0; i < n; i++)
        {
            var msg = Marshal.PtrToStructure<SteamNetworkingMessage_t>(ptrs[i]);

            byte[] data = new byte[msg.m_cbSize];
            Marshal.Copy(msg.m_pData, data, 0, msg.m_cbSize);

            // 반드시 Release
            SteamNetworkingMessage_t.Release(ptrs[i]);

            string text = Encoding.UTF8.GetString(data);
            Debug.Log($"[CLIENT RECV] {text}");
        }
    }

    void OnDestroy()
    {
        if (_conn != HSteamNetConnection.Invalid)
        {
            SteamNetworkingSockets.CloseConnection(_conn, 0, "client quit", false);
            _conn = HSteamNetConnection.Invalid;
        }
    }
}