using System.Net.Sockets;
using UnityEngine;

public static class MapGimicBuilder
{
    public static string BuildRat_SyncHP(int gimicId, int targetHP)
    {
        Packet_1_Rat msg = new Packet_1_Rat
        {
            gimicId = gimicId,
            targetHpSync = targetHP
        };
        return "[GIMIC_RAT]" + JsonUtility.ToJson(msg);
    }

    public static string Build_Cow_Earthquake()
    {
        Packet_2_Cow msg = new Packet_2_Cow
        {
            gimicId = 2,   // 소 기믹 ID
            actionType = 1 // 1번을 지진으로 약속
        };
        return "[GIMIC_COW]" + JsonUtility.ToJson(msg);
    }

    public static string BuildTiger(int gimicId, bool isStart)
    {
        Packet_3_Tiger msg = new Packet_3_Tiger
        {
            gimicId = gimicId,
            isStart = isStart
        };
        return "[GIMIC_TIGER]" + JsonUtility.ToJson(msg);
    }

    public static string BuildRabbit(int id, bool isStart)
    {
        Packet_4_Rabbit msg = new Packet_4_Rabbit
        {
            gimicId = id,
            isStart = isStart
        };
        return "[GIMIC_RABBIT]" + JsonUtility.ToJson(msg);
    }

    public static string BuildDragon_Spawn(int id, float x, float y, float z, float angleZ)
    {
        Packet_5_Dragon msg = new Packet_5_Dragon
        {
            gimicId = id,
            x = x,
            y = y,
            z = z,
            angleZ = angleZ
        };
        return "[GIMIC_DRAGON]" + JsonUtility.ToJson(msg);
    }


    public static string BuildSnake_Spawn(int id, float x, float y, float z)
    {
        Packet_6_Snake msg = new Packet_6_Snake
        {
            gimicId = id,
            x = x,
            y = y,
            z = z
        };
        return "[GIMIC_SNAKE]" + JsonUtility.ToJson(msg);
    }

    public static string BuildHorse(int id, bool isStart)
    {
        Packet_7_Horse msg = new Packet_7_Horse { gimicId = id, isStart = isStart };
        return "[GIMIC_HORSE]" + JsonUtility.ToJson(msg);
    }

    public static string BuildSheep_Spawn(int id, float x, float y, float z)
    {
        Packet_8_Sheep msg = new Packet_8_Sheep
        {
            gimicId = id,
            x = x,
            y = y,
            z = z
        };
        return "[GIMIC_SHEEP]" + JsonUtility.ToJson(msg);
    }



    public static string BuildMonkey(int id)
    {
        Packet_9_Monkey msg = new Packet_9_Monkey
        {
            gimicId = id
        };
        return "[GIMIC_MONKEY]" + JsonUtility.ToJson(msg);
    }

    public static string BuildChicken_Spawn(int id, float ratio, float rotZ)
    {
        Packet_10_Chicken msg = new Packet_10_Chicken
        {
            gimicId = id,
            spawnRatio = ratio,
            randomRotationZ = rotZ
        };
        return "[GIMIC_CHICKEN]" + JsonUtility.ToJson(msg);
    }





    public static string BuildDog(int id, bool isActive)
    {
        Packet_11_Dog msg = new Packet_11_Dog { gimicId = id, isActive = isActive };
        return "[GIMIC_DOG]" + JsonUtility.ToJson(msg);
    }


    public static string BuildPig_Spawn(int id, float x, float y, float z)
    {
        Packet_12_Pig msg = new Packet_12_Pig
        {
            gimicId = id,
            x = x,
            y = y,
            z = z
        };
        return "[GIMIC_PIG]" + JsonUtility.ToJson(msg);
    }







    #region
    public static string BuildScreen_Reset()
    {
        Packet_ScreenEffect msg = new Packet_ScreenEffect { effectType = 0 };
        return "[GIMIC_SCREEN]" + JsonUtility.ToJson(msg);
    }

    public static string BuildScreen_Color(Color c)
    {
        Packet_ScreenEffect msg = new Packet_ScreenEffect
        {
            effectType = 1,
            r = c.r,
            g = c.g,
            b = c.b
        };
        return "[GIMIC_SCREEN]" + JsonUtility.ToJson(msg);
    }

    public static string BuildScreen_Tween(float duration, float targetThick)
    {
        Packet_ScreenEffect msg = new Packet_ScreenEffect
        {
            effectType = 2,
            duration = duration,
            targetThick = targetThick
        };
        return "[GIMIC_SCREEN]" + JsonUtility.ToJson(msg);
    }
    #endregion




}
