using UnityEngine;




[System.Serializable]
public class Model_MapGimic
{
    public int gimicId;
}



[System.Serializable]
public class Packet_1_Rat : Model_MapGimic
{
    public int targetHpSync;
}


[System.Serializable]
public class Packet_2_Cow : Model_MapGimic
{
    // 1: 지진(Earthquake) 실행
    public int actionType;
}




[System.Serializable]
public class Packet_3_Tiger : Model_MapGimic
{
    public bool isStart; // true: 시작(Execute), false: 끝(Undo)
}



[System.Serializable]
public class Packet_4_Rabbit : Model_MapGimic
{
    public bool isStart; // Tiger와 동일하게 추가
}


[System.Serializable]
public class Packet_5_Dragon : Model_MapGimic
{
    public float x, y, z;
    public float angleZ; // 회전 각도
}



[System.Serializable]
public class Packet_6_Snake : Model_MapGimic
{
    public float x;
    public float y;
    public float z;
}




[System.Serializable]
public class Packet_7_Horse : Model_MapGimic
{
    public bool isStart;
}


[System.Serializable]
public class Packet_8_Sheep : Model_MapGimic
{
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public class Packet_9_Monkey : Model_MapGimic
{
    // 위치 정보를 담거나 단순 실행 신호만 보냄
}


[System.Serializable]
public class Packet_10_Chicken : Model_MapGimic
{
    public float spawnRatio; // 0.0(왼쪽) ~ 1.0(오른쪽) 사이 값
    public float randomRotationZ; // 랜덤 회전 각도
}



[System.Serializable]
public class Packet_11_Dog : Model_MapGimic
{
    public bool isActive;
}

[System.Serializable]
public class Packet_12_Pig : Model_MapGimic
{
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public class Packet_ScreenEffect : Model_MapGimic
{
    public int effectType; // 0: Reset, 1: SetColor, 2: TweenBorder
    public float duration;
    public float targetThick;
    public float r, g, b; // Color RGB
}