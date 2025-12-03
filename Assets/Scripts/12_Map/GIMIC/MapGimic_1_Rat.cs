using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 일정시간 마다 플레이어의 체력을 더해 동일하게 반으로 나눕니다. 

public class MapGimic_1_Rat : AbstractMapGimic
{

    public override void OnGimicStart()
    {
        base.OnGimicStart();  
     
    }

    public override void OnGimmickUpdate()
    {
        base.OnGimmickUpdate();

        if (true)
        {
            //// ★ 핵심: 색상과 함께 '함수 이름'을 그냥 넣으세요.
            mapManager.PlayBorderAnimation(new Color(1f, 0.8f, 0.8f), ExFunc);
        }
    }

    public override void OnGimicEnd()
    {
        base.OnGimicEnd();   
    }

    public void ExFunc()
    {

    }



}
