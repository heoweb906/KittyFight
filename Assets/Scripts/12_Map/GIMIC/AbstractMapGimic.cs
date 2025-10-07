using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractMapGimic : MonoBehaviour
{
    public GameManager gameManger;
    public MapManager mapManager;
    protected bool bIsActive = false;

    public virtual void OnGimicStart()
    {
        bIsActive = true;

        // ���� �ʿ�
        switch (mapManager.currentIndex)
        {
            case 1:
                mapManager.iamge_TestMapGimicColor.color = Color.red; // ����
                break;
            case 2:
                mapManager.iamge_TestMapGimicColor.color = new Color(1f, 0.5f, 0f); // ��Ȳ
                break;
            case 3:
                mapManager.iamge_TestMapGimicColor.color = Color.yellow; // ���
                break;
            case 4:
                mapManager.iamge_TestMapGimicColor.color = Color.green; // �ʷ�
                break;
            case 5:
                mapManager.iamge_TestMapGimicColor.color = Color.cyan; // �ϴû�
                break;
            case 6:
                mapManager.iamge_TestMapGimicColor.color = Color.blue; // �Ķ�
                break;
            case 7:
                mapManager.iamge_TestMapGimicColor.color = new Color(0.5f, 0f, 1f); // ����
                break;
            case 8:
                mapManager.iamge_TestMapGimicColor.color = new Color(0.5f, 0f, 0.5f); // ����
                break;
            case 9:
                mapManager.iamge_TestMapGimicColor.color = Color.magenta; // ��ȫ
                break;
            case 10:
                mapManager.iamge_TestMapGimicColor.color = new Color(1f, 0.75f, 0.8f); // ��ȫ
                break;
            case 11:
                mapManager.iamge_TestMapGimicColor.color = new Color(0.6f, 0.4f, 0.2f); // ����
                break;
            case 12:
                mapManager.iamge_TestMapGimicColor.color = Color.white; // ���
                break;
        }
    }



    public virtual void OnGimmickUpdate() { }



    public virtual void OnGimicEnd()
    {
        bIsActive = false;

        mapManager.iamge_TestMapGimicColor.color = Color.black;

    }

}
