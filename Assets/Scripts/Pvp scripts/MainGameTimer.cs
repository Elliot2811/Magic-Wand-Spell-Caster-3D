using System.Collections;
using UnityEngine;

public class MainGameTimer : MonoBehaviour
{
    public float mainGameTime = 90;
    private float currentTime = 0;
    private int lastWholeSecond = 0;
    private bool isTimerRunning = false;

    private void OnEnable()
    {
        TempGameManager.StartMainGame += StartMainGameTimer;
    }

    private void OnDisable()
    {
        TempGameManager.StartMainGame -= StartMainGameTimer;
    }

    private void Update()
    {
        if (!isTimerRunning) return;

        //Update display timer
        int displayTime = Mathf.CeilToInt(currentTime);
        if (displayTime != lastWholeSecond)
        {
            lastWholeSecond = displayTime;
            Debug.Log("Timer: " + displayTime);
        }

        //Internal logic timer
        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isTimerRunning = false;

            Debug.Log("Time's up!");
        }
    }

    private void StartMainGameTimer(TempGameManager.GameRunStatus state)
    {
        //Only runs the rest of the function's code if enum value = GameStarted
        if (state != TempGameManager.GameRunStatus.GameStarted)
                return;

        currentTime = mainGameTime;
        isTimerRunning = true;
    }
}
