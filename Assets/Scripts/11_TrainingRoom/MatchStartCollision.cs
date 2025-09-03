using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchStartCollision : MonoBehaviour
{
    public MainMenuController mainMenuController { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mainMenuController.StartMatching();
        }
    }


}
