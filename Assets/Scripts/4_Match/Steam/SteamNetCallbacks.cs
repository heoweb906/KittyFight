using Steamworks;
using UnityEngine;

public class SteamNetCallbacks : MonoBehaviour
{
    private Callback<SteamNetConnectionStatusChangedCallback_t> _cb;

    void Awake()
    {
        _cb = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnConnStatusChanged);
    }

    private void OnConnStatusChanged(SteamNetConnectionStatusChangedCallback_t data)
    {
        Debug.Log($"[SNET] ConnStatusChanged: conn={data.m_hConn} " +
                  $"old={data.m_eOldState} new={data.m_info.m_eState} " +
                  $"endReason={data.m_info.m_eEndReason} " +
                  $"debug={data.m_info.m_szEndDebug}");
    }
}