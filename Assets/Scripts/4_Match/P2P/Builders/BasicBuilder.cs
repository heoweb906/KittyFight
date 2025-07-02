using UnityEngine;

public static class BasicBuilder
{
    public static string Build(int _iPlayerNum, string sModel)
    {
        Model_Basic msg = new Model_Basic
        {
            iPlayer = _iPlayerNum,
        };

        return sModel + JsonUtility.ToJson(msg);
    }

}
