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

            GameObject.FindObjectOfType<GameManager>().EndGame();
            Debug.Log("게임 종료!");
        }

        timerText.text = $"{Mathf.FloorToInt(remainingTime)}";
    }
}