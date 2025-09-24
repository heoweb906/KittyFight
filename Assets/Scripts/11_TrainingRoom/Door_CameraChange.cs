using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Door_CameraChange : MonoBehaviour
{
    public int iCameraIndex;
    public TitleLogoAssist titleLogoAssist { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            titleLogoAssist.ChangeVirtualCamera(iCameraIndex);
        }
    }
}