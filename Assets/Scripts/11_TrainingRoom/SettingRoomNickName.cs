using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SettingRoomNickName : MonoBehaviour
{
    public MainMenuController mainMenuController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mainMenuController.OnNickNameInputPanel();
            mainMenuController.ResetPlayerPosition(other.gameObject);
        }
    }
   
}