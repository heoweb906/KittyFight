using UnityEngine;
using Steamworks;

public class SteamBootstrap : MonoBehaviour
{
    void Awake()
    {
        // 중복 방지
        DontDestroyOnLoad(gameObject);

        if (!Packsize.Test() || !DllCheck.Test())
        {
            Debug.LogError("Steamworks.NET packsize/dll check failed.");
            return;
        }

        bool ok = SteamAPI.Init();
        Debug.Log("SteamAPI.Init = " + ok);

        if (ok)
        {
            Debug.Log("SteamID = " + SteamUser.GetSteamID());
            Debug.Log("PersonaName = " + SteamFriends.GetPersonaName());
        }
    }

    void Update()
    {
        // 콜백 펌프 (필수)
        if (SteamManager.Initialized) SteamAPI.RunCallbacks();
    }

    void OnApplicationQuit()
    {
        if (SteamManager.Initialized) SteamAPI.Shutdown();
    }
}