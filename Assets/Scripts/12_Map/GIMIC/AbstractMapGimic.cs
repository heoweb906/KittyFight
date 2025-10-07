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

        // ¼öÁ¤ ÇÊ¿ä
        switch (mapManager.currentIndex)
        {
            case 1:
                mapManager.iamge_TestMapGimicColor.color = Color.red; // »¡°­
                break;
            case 2:
                mapManager.iamge_TestMapGimicColor.color = new Color(1f, 0.5f, 0f); // ÁÖÈ²
                break;
            case 3:
                mapManager.iamge_TestMapGimicColor.color = Color.yellow; // ³ë¶û
                break;
            case 4:
                mapManager.iamge_TestMapGimicColor.color = Color.green; // ÃÊ·Ï
                break;
            case 5:
                mapManager.iamge_TestMapGimicColor.color = Color.cyan; // ÇÏ´Ã»ö
                break;
            case 6:
                mapManager.iamge_TestMapGimicColor.color = Color.blue; // ÆÄ¶û
                break;
            case 7:
                mapManager.iamge_TestMapGimicColor.color = new Color(0.5f, 0f, 1f); // ³²»ö
                break;
            case 8:
                mapManager.iamge_TestMapGimicColor.color = new Color(0.5f, 0f, 0.5f); // º¸¶ó
                break;
            case 9:
                mapManager.iamge_TestMapGimicColor.color = Color.magenta; // ÀÚÈ«
                break;
            case 10:
                mapManager.iamge_TestMapGimicColor.color = new Color(1f, 0.75f, 0.8f); // ºÐÈ«
                break;
            case 11:
                mapManager.iamge_TestMapGimicColor.color = new Color(0.6f, 0.4f, 0.2f); // °¥»ö
                break;
            case 12:
                mapManager.iamge_TestMapGimicColor.color = Color.white; // Èò»ö
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
