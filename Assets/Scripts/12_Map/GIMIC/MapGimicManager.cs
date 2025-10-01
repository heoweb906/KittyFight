using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGimicManager : MonoBehaviour
{
    public GameManager gameManager;

    [SerializeField] private List<AbstractMapGimic> gimicks;
    private AbstractMapGimic currentGimmick;
    private int currentIndex = -1;


    private void Start()
    {
        for(int i = 0; i < gimicks.Count;++i) gimicks[i].gameManger = gameManager;
        ActivateGimmick(0);
    }



    private void FixedUpdate()
    {
        if (currentGimmick != null)
            currentGimmick.OnGimmickUpdate();
    }



    public void ActivateGimmick(int index)
    {
        if (currentGimmick != null) currentGimmick.OnGimicEnd();

        index--;


        if (index < 0 || index >= gimicks.Count)
        {
            currentGimmick = null;
            currentIndex = -1;
            return;
        }

        currentIndex = index;
        currentGimmick = gimicks[index];
        currentGimmick.OnGimicStart();
    }


    public void StopCurrentGimmick()
    {
        if (currentGimmick != null)
        {
            currentGimmick.OnGimicEnd();
            currentGimmick = null;
            currentIndex = -1;
        }
    }
}