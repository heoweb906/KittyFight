using System.IO;
using UnityEngine;

public class SteamPathDebug : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("Application.dataPath = " + Application.dataPath);
        Debug.Log("CurrentDirectory = " + Directory.GetCurrentDirectory());
        Debug.Log("steam_appid.txt exists = " + File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "steam_appid.txt")));
    }
}