using UnityEngine;

[System.Serializable]
public class Model_MapGimic
{
    public int gimicId;
}

[System.Serializable]
public class Packet_1_Rat : Model_MapGimic
{
    public int targetHpSync; // ★ 추가: 동기화할 목표 체력
}

[System.Serializable]
public class Packet_2_Cow : Model_MapGimic
{
     
}

[System.Serializable]
public class Packet_3_Tiger : Model_MapGimic
{

}

[System.Serializable]
public class Packet_4_Rabbit : Model_MapGimic
{

}

[System.Serializable]
public class Packet_5_Dragon : Model_MapGimic
{

}

[System.Serializable]
public class Packet_6_Snake : Model_MapGimic
{

}
