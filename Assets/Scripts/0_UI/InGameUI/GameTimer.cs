using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TMP_Text timerText;
    private float remainingTime = 0f;
    private bool isRunning = false;

    public void SetDuration(float duration)
    {
        remainingTime = duration;
        isRunning = true;
    }

    public bool Tick(float deltaTime)
    {
        if (!isRunning) return false;

        remainingTime -= deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isRunning = false;
            timerText.text = "0";
            return true;
        }

        timerText.text = $"{Mathf.FloorToInt(remainingTime)}";
        return false;
    }
}