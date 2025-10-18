using UnityEngine;

[System.Serializable]
public class DamageMessage
{
    public int targetPlayer;   // �ǰݵ� �÷��̾� ��ȣ
    public int hp;             // ���� HP

    public bool hasSource;     // ��ǥ ���� ���� (���� ����)
    public Vector3 sourceWorldPos;
}