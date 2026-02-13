using System;
using System.Runtime.InteropServices;
using System.Text;
using Steamworks;
using UnityEngine;

public static class SteamP2PManager
{
    public enum Role { None, Host, Client }

    public static bool IsConnected => _conn != HSteamNetConnection.Invalid;

    private static Role _role = Role.None;
    private static int _virtualPort = 0;
    private static bool _reliable = true;

    private static HSteamListenSocket _listen = HSteamListenSocket.Invalid;
    private static HSteamNetConnection _conn = HSteamNetConnection.Invalid;
    private static Callback<SteamNetConnectionStatusChangedCallback_t> _cbConn;

    public static void Init(Role role, ulong opponentSteamId, int virtualPort = 0, bool reliable = true)
    {
        Dispose(); // 재진입 대비

        if (!SteamManager.Initialized)
        {
            Debug.LogError("[SteamP2PManager] SteamManager.Initialized == false");
            return;
        }

        _role = role;
        _virtualPort = virtualPort;
        _reliable = reliable;

        SteamNetworkingUtils.InitRelayNetworkAccess();
        _cbConn = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnConnStatusChanged);

        if (_role == Role.Host)
        {
            _listen = SteamNetworkingSockets.CreateListenSocketP2P(_virtualPort, 0, null);
            Debug.Log($"[SteamP2PManager] HOST Listen opened. vPort={_virtualPort}, listen={_listen}");
        }
        else if (_role == Role.Client)
        {
            var identity = new SteamNetworkingIdentity();
            identity.SetSteamID(new CSteamID(opponentSteamId));

            _conn = SteamNetworkingSockets.ConnectP2P(ref identity, _virtualPort, 0, null);
            Debug.Log($"[SteamP2PManager] CLIENT ConnectP2P => conn={_conn}, host={opponentSteamId}, vPort={_virtualPort}");
        }
    }

    // Steam 수신 폴링 (GameManager.Update에서 매 프레임 호출)
    public static void PollReceive()
    {
        if (_conn == HSteamNetConnection.Invalid) return;

        IntPtr[] ptrs = new IntPtr[32];
        int n = SteamNetworkingSockets.ReceiveMessagesOnConnection(_conn, ptrs, ptrs.Length);

        for (int i = 0; i < n; i++)
        {
            var msg = Marshal.PtrToStructure<SteamNetworkingMessage_t>(ptrs[i]);

            byte[] data = new byte[msg.m_cbSize];
            Marshal.Copy(msg.m_pData, data, 0, msg.m_cbSize);

            SteamNetworkingMessage_t.Release(ptrs[i]);

            string text = Encoding.UTF8.GetString(data);
            //Debug.Log("[SteamP2P] recv: " + text);
            // UDP와 동일하게 Dispatcher로
            P2PMessageDispatcher.Dispatch(text);
        }
    }

    public static void SendRaw(string msg)
    {
        if (_conn == HSteamNetConnection.Invalid) return;
        if (string.IsNullOrEmpty(msg)) return;

        byte[] bytes = Encoding.UTF8.GetBytes(msg);

        int sendFlags = _reliable
            ? Constants.k_nSteamNetworkingSend_Reliable
            : Constants.k_nSteamNetworkingSend_Unreliable;

        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            var r = SteamNetworkingSockets.SendMessageToConnection(
                _conn,
                handle.AddrOfPinnedObject(),
                (uint)bytes.Length,
                sendFlags,
                out long _);

            if (r != EResult.k_EResultOK)
                Debug.LogWarning($"[SteamP2PManager] Send failed: {r}");
        }
        finally { handle.Free(); }
    }

    private static void OnConnStatusChanged(SteamNetConnectionStatusChangedCallback_t data)
    {
        var newState = data.m_info.m_eState;

        Debug.Log($"[SteamP2PManager] StatusChanged conn={data.m_hConn} old={data.m_eOldState} new={newState} " +
                  $"endReason={data.m_info.m_eEndReason} debug={data.m_info.m_szEndDebug}");

        // Host는 Connecting에서 Accept 해야 함
        if (_role == Role.Host && newState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting)
        {
            var r = SteamNetworkingSockets.AcceptConnection(data.m_hConn);
            Debug.Log($"[SteamP2PManager] HOST AcceptConnection({data.m_hConn}) => {r}");

            if (r == EResult.k_EResultOK)
            {
                _conn = data.m_hConn; // 단일 연결 가정
                Debug.Log($"[SteamP2PManager] HOST Active conn={_conn}");
            }
        }

        if (newState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected)
        {
            P2PManager.IsReadyToStartGame = true;
            Debug.Log("[Steam] Connected -> P2PManager.IsReadyToStartGame = true");
        }

        // 종료/문제
        if (newState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer ||
            newState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally)
        {
            if (_conn == data.m_hConn) _conn = HSteamNetConnection.Invalid;
            SteamNetworkingSockets.CloseConnection(data.m_hConn, 0, "closed", false);
        }
    }

    public static void Dispose()
    {
        if (_conn != HSteamNetConnection.Invalid)
        {
            SteamNetworkingSockets.CloseConnection(_conn, 0, "dispose", false);
            _conn = HSteamNetConnection.Invalid;
        }

        if (_listen != HSteamListenSocket.Invalid)
        {
            SteamNetworkingSockets.CloseListenSocket(_listen);
            _listen = HSteamListenSocket.Invalid;
        }

        _cbConn = null;
        _role = Role.None;
    }
}