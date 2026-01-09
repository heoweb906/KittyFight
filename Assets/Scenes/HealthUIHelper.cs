using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestDebug : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private void FixedUpdate()
    {
        Debug.Log(fillImage.fillAmount);
    }
}
