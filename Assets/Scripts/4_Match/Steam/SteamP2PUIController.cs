using System;
using System.Runtime.InteropServices;
using System.Text;
using Steamworks;
using TMPro;
using UnityEngine;

public class SteamP2PUIController : MonoBehaviour
{
    public enum Role { None, Host, Client }

    [Header("UI")]
    public UILogger logger;
    public TMP_InputField hostSteamIdInput;
    public TMP_InputField messageInput;

    [Header("P2P")]
    public int virtualPort = 0;
    public bool reliable = true;

    private Role _role = Role.None;
    private HSteamListenSocket _listen = HSteamListenSocket.Invalid;
    private HSteamNetConnection _conn = HSteamNetConnection.Invalid;
    private Callback<SteamNetConnectionStatusChangedCallback_t> _cbConn;

    void Awake()
    {
        if (logger == null) logger = FindFirstObjectByType<UILogger>();

        if (!SteamManager.Initialized)
        {
            Log("[Steam] SteamManager.Initialized == false. SteamAPI.Init() 먼저 성공해야 함.");
            enabled = false;
            return;
        }

        SteamNetworkingUtils.InitRelayNetworkAccess();
        _cbConn = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnConnStatusChanged);

        Log($"[Steam] MySteamID = {SteamUser.GetSteamID()}");
        Log($"[Steam] Persona = {SteamFriends.GetPersonaName()}");
    }

    void Update()
    {
        // 연결이 잡혔으면 계속 수신
        if (_conn != HSteamNetConnection.Invalid)
        {
            ReceiveLoop(_conn);
        }
    }

    // ---------- UI Buttons ----------

    public void OnClickBeHost()
    {
        Cleanup();

        _role = Role.Host;
        _listen = SteamNetworkingSockets.CreateListenSocketP2P(virtualPort, 0, null);
        Log($"[HOST] Listen opened. vPort={virtualPort}, listen={_listen}");
        Log("[HOST] 상대가 ConnectP2P 하면 Connecting 콜백이 오고, 여기서 AcceptConnection 함.");
    }

    public void OnClickBeClientAndConnect()
    {
        Cleanup();
        _role = Role.Client;

        if (hostSteamIdInput == null || string.IsNullOrWhiteSpace(hostSteamIdInput.text))
        {
            Log("[CLIENT] hostSteamIdInput 비었음.");
            return;
        }

        if (!ulong.TryParse(hostSteamIdInput.text.Trim(), out ulong hostId) || hostId == 0)
        {
            Log("[CLIENT] hostSteamId 파싱 실패.");
            return;
        }

        var identity = new SteamNetworkingIdentity();
        identity.SetSteamID(new CSteamID(hostId));

        _conn = SteamNetworkingSockets.ConnectP2P(ref identity, virtualPort, 0, null);
        Log($"[CLIENT] ConnectP2P => conn={_conn}, host={hostId}, vPort={virtualPort}");
    }

    public void OnClickSendMessage()
    {
        if (_conn == HSteamNetConnection.Invalid)
        {
            Log("[SEND] 연결 없음.");
            return;
        }

        string msg = messageInput != null ? messageInput.text : "";
        if (string.IsNullOrWhiteSpace(msg))
            msg = $"Hello @ {DateTime.Now:HH:mm:ss.fff}";

        bool ok = SendString(_conn, msg, reliable);
        Log(ok ? $"[SEND] {msg}" : "[SEND] failed");
    }

    public void OnClickDisconnect()
    {
        Log("[UI] Disconnect");
        Cleanup();
        _role = Role.None;
    }

    // ---------- Steam callbacks / networking ----------

    private void OnConnStatusChanged(SteamNetConnectionStatusChangedCallback_t data)
    {
        var newState = data.m_info.m_eState;
        Log($"[SNET] StatusChanged conn={data.m_hConn} old={data.m_eOldState} new={newState} " +
            $"endReason={data.m_info.m_eEndReason} debug={data.m_info.m_szEndDebug}");

        // Host는 들어오는 연결을 Accept 해야 함
        if (_role == Role.Host && newState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting)
        {
            var r = SteamNetworkingSockets.AcceptConnection(data.m_hConn);
            Log($"[HOST] AcceptConnection({data.m_hConn}) => {r}");

            if (r == EResult.k_EResultOK)
            {
                _conn = data.m_hConn; // 단일 연결 가정
                Log($"[HOST] Active conn = {_conn}");
            }
        }

        // 종료/문제 처리 (Host/Client 공통)
        if (newState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer ||
            newState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally)
        {
            Log("[SNET] Closed/Problem. Cleaning up.");
            if (_conn == data.m_hConn) _conn = HSteamNetConnection.Invalid;
            SteamNetworkingSockets.CloseConnection(data.m_hConn, 0, "closed", false);
        }
    }

    private static bool SendString(HSteamNetConnection conn, string s, bool reliable)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(s);

        int sendFlags = reliable
            ? Constants.k_nSteamNetworkingSend_Reliable
            : Constants.k_nSteamNetworkingSend_Unreliable;

        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            var r = SteamNetworkingSockets.SendMessageToConnection(
                conn,
                handle.AddrOfPinnedObject(),
                (uint)bytes.Length,
                sendFlags,
                out long _);

            return r == EResult.k_EResultOK;
        }
        finally { handle.Free(); }
    }

    private void ReceiveLoop(HSteamNetConnection conn)
    {
        IntPtr[] ptrs = new IntPtr[32];
        int n = SteamNetworkingSockets.ReceiveMessagesOnConnection(conn, ptrs, ptrs.Length);

        for (int i = 0; i < n; i++)
        {
            var msg = Marshal.PtrToStructure<SteamNetworkingMessage_t>(ptrs[i]);

            byte[] data = new byte[msg.m_cbSize];
            Marshal.Copy(msg.m_pData, data, 0, msg.m_cbSize);

            SteamNetworkingMessage_t.Release(ptrs[i]);

            string text = Encoding.UTF8.GetString(data);
            Log($"[RECV] {text}");
        }
    }

    private void Cleanup()
    {
        if (_conn != HSteamNetConnection.Invalid)
        {
            SteamNetworkingSockets.CloseConnection(_conn, 0, "ui disconnect", false);
            _conn = HSteamNetConnection.Invalid;
        }

        if (_listen != HSteamListenSocket.Invalid)
        {
            SteamNetworkingSockets.CloseListenSocket(_listen);
            _listen = HSteamListenSocket.Invalid;
        }
    }

    private void Log(string s)
    {
        if (logger != null) logger.AppendLog(s);
        else Debug.Log(s);
    }

    void OnDestroy()
    {
        Cleanup();
    }
}