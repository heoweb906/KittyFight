using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TMP_Text timerText;
    private float remainingTime = 0f;
    private bool isRunning = false;

    public void StartTimer(float duration)
    {
        remainingTime = duration;
        isRunning = true;
    }

    private void Update()
    {
        if (!isRunning) return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isRunning = false;

            Debug.Log("게임 종료!");
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = $"{minutes:D2}:{seconds:D2}";
    }
}