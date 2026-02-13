using Steamworks;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Text;

public class SteamP2PHost : MonoBehaviour
{
    [Header("P2P Settings")]
    public int virtualPort = 0;

    [Header("Send Test")]
    public KeyCode sendKey = KeyCode.Space;
    public bool reliable = true;

    private HSteamListenSocket _listen = HSteamListenSocket.Invalid;
    private HSteamNetConnection _conn = HSteamNetConnection.Invalid;

    private Callback<SteamNetConnectionStatusChangedCallback_t> _cbConnStatus;

    void Awake()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("[HOST] SteamManager not initialized. SteamAPI.Init() must succeed first.");
            enabled = false;
            return;
        }

        // 연결 상태 변경 콜백 등록
        _cbConnStatus = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnConnStatusChanged);
    }

    void Start()
    {
        // SDR(릴레이) 접근 준비 - NAT와 무관하게 성립시키는 쪽
        SteamNetworkingUtils.InitRelayNetworkAccess();

        // Listen socket 열기
        _listen = SteamNetworkingSockets.CreateListenSocketP2P(virtualPort, 0, null);

        Debug.Log($"[HOST] ListenSocket opened. listen={_listen}, vPort={virtualPort}, mySteamId={SteamUser.GetSteamID()}");
    }

    void Update()
    {
        if (_conn != HSteamNetConnection.Invalid)
        {
            // 테스트 송신 (호스트도 메시지 보내기)
            if (Input.GetKeyDown(sendKey))
            {
                string msg = $"Hello from host @ {DateTime.Now:HH:mm:ss.fff}";
                bool ok = SendString(_conn, msg, reliable);
                Debug.Log(ok ? $"[HOST] Sent: {msg}" : "[HOST] Send failed.");
            }

            // 수신 루프
            ReceiveLoop(_conn);
        }
    }

    private void OnConnStatusChanged(SteamNetConnectionStatusChangedCallback_t data)
    {
        // 이 콜백은 "리스닝 소켓으로 들어온 연결"도 여기로 들어옴
        var newState = data.m_info.m_eState;

        Debug.Log($"[HOST] ConnStatusChanged: conn={data.m_hConn} old={data.m_eOldState} new={newState} " +
                  $"endReason={data.m_info.m_eEndReason} debug={data.m_info.m_szEndDebug}");

        switch (newState)
        {
            case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:
                {
                    // 들어오는 연결을 수락(Accept)
                    // Host는 여기서 AcceptConnection을 호출해야 연결이 진행됨
                    var r = SteamNetworkingSockets.AcceptConnection(data.m_hConn);
                    Debug.Log($"[HOST] AcceptConnection({data.m_hConn}) => {r}");

                    if (r == EResult.k_EResultOK)
                    {
                        // 단일 세션만 가정 (여러 명이면 conn 리스트/딕셔너리로 확장)
                        _conn = data.m_hConn;
                        Debug.Log($"[HOST] Active connection set: {_conn}");
                    }
                    break;
                }

            case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
                Debug.Log("[HOST] Connected.");
                break;

            case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
            case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
                {
                    Debug.LogWarning("[HOST] Connection closed/problem. Cleaning up.");
                    if (_conn == data.m_hConn) _conn = HSteamNetConnection.Invalid;

                    SteamNetworkingSockets.CloseConnection(data.m_hConn, 0, "closed", false);
                    break;
                }
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
            EResult r = SteamNetworkingSockets.SendMessageToConnection(
                conn,
                handle.AddrOfPinnedObject(),
                (uint)bytes.Length,
                sendFlags,
                out long _);

            if (r != EResult.k_EResultOK)
            {
                Debug.LogError($"[HOST] SendMessageToConnection failed: {r}");
                return false;
            }
            return true;
        }
        finally
        {
            handle.Free();
        }
    }

    private static void ReceiveLoop(HSteamNetConnection conn)
    {
        IntPtr[] ptrs = new IntPtr[16];
        int n = SteamNetworkingSockets.ReceiveMessagesOnConnection(conn, ptrs, ptrs.Length);

        for (int i = 0; i < n; i++)
        {
            var msg = Marshal.PtrToStructure<SteamNetworkingMessage_t>(ptrs[i]);

            byte[] data = new byte[msg.m_cbSize];
            Marshal.Copy(msg.m_pData, data, 0, msg.m_cbSize);

            SteamNetworkingMessage_t.Release(ptrs[i]);

            string text = Encoding.UTF8.GetString(data);
            Debug.Log($"[HOST RECV] {text}");
        }
    }

    void OnDestroy()
    {
        if (_conn != HSteamNetConnection.Invalid)
        {
            SteamNetworkingSockets.CloseConnection(_conn, 0, "host quit", false);
            _conn = HSteamNetConnection.Invalid;
        }

        if (_listen != HSteamListenSocket.Invalid)
        {
            SteamNetworkingSockets.CloseListenSocket(_listen);
            _listen = HSteamListenSocket.Invalid;
        }
    }
}