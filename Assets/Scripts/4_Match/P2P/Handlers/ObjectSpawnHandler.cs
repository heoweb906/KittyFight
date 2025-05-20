using UnityEngine;

public class ObjectSpawnHandler : IP2PMessageHandler
{
    private GameObject player1;
    private GameObject player2;

    public ObjectSpawnHandler(GameObject player1Obj, GameObject player2Obj)
    {
        player1 = player1Obj;
        player2 = player2Obj;
    }

    public bool CanHandle(string msg) => msg.StartsWith("[SPAWN]");

    public void Handle(string msg)
    {
        string json = msg.Substring(7);
        var data = JsonUtility.FromJson<SpawnMessage>(json);
        Vector3 pos = new Vector3(data.x, data.y, data.z);

        GameObject senderPlayer = (data.player == 1) ? player1 : player2;
        if (senderPlayer == null) return;

        var moveScript = senderPlayer.GetComponent<PlayerMove>();
        if (moveScript == null || moveScript.spawnPrefab == null) return;

        GameObject spawned = Object.Instantiate(moveScript.spawnPrefab, pos, Quaternion.identity);
    }
}