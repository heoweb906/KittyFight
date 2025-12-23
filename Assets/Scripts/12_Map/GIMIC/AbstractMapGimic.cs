using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractMapGimic : MonoBehaviour
{
    public GameManager gameManager;
    public MapManager mapManager;
    protected bool bIsActive = false;


    // 테스트용 !!!!
    // 테스트용 !!!!
    protected float flashTimer = 0f;
    protected bool isWhitePhase = false;
    protected Color originColor; // 원래 기믹의 색상 저장용
    // 테스트용 !!!!
    // 테스트용 !!!!

    public virtual void OnGimicStart()
    {
        bIsActive = true;

        // 수정 필요
        switch (mapManager.GetMapGimicIndex())
        {
            case 1: // 쥐
                mapManager.iamge_TestMapGimicColor.color = Color.red; // 빨강
                break;
            case 2:  // 소
                mapManager.iamge_TestMapGimicColor.color = new Color(1f, 0.5f, 0f); // 주황
                break;
            case 3:  // 호랑이
                mapManager.iamge_TestMapGimicColor.color = Color.yellow; // 노랑
                break;
            case 4:  // 토끼
                mapManager.iamge_TestMapGimicColor.color = Color.green; // 초록
                break;
            case 5:  // 용
                mapManager.iamge_TestMapGimicColor.color = Color.cyan; // 하늘색
                break;
            case 6:  // 뱀
                mapManager.iamge_TestMapGimicColor.color = Color.blue; // 파랑
                break;
            case 7:  // 말
                mapManager.iamge_TestMapGimicColor.color = new Color(0.5f, 0f, 1f); // 남색
                break;
            case 8:  // 양
                mapManager.iamge_TestMapGimicColor.color = new Color(0.5f, 0f, 0.5f); // 보라
                break;
            case 9:  // 원숭이
                mapManager.iamge_TestMapGimicColor.color = Color.magenta; // 자홍
                break;
            case 10: // 닭
                mapManager.iamge_TestMapGimicColor.color = new Color(1f, 0.75f, 0.8f); // 분홍
                break;
            case 11: // 개
                mapManager.iamge_TestMapGimicColor.color = new Color(0.6f, 0.4f, 0.2f); // 갈색
                break;
            case 12: // 돼지
                mapManager.iamge_TestMapGimicColor.color = Color.white; // 흰색
                break;
        }
    }


    public virtual void OnGimmickUpdate() { }


    public virtual void OnGimicEnd()
    {
        bIsActive = false;

        mapManager.iamge_TestMapGimicColor.color = Color.black;

        mapManager.ResetScreenEffect();
    }


    public virtual void OnReceivePacket(Model_MapGimic packet) { }

    public void SendTween(float duration, float thick)
    {
        mapManager.TweenScreenBorder(duration, thick);
        P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Tween(duration, thick));
    }


}


