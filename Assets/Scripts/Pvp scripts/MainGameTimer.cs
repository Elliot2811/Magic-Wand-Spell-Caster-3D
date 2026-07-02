//using System.Collections;
//using UnityEngine;

//public class MainGameTimer : MonoBehaviour
//{
//    #region Variables
//    [Header("Timer Settings")]
//    public float mainGameTime = 90;
//    private float currentTime = 0;
//    private int lastWholeSecond = 0;
//    private bool isTimerRunning = false;
//    #endregion

//    #region Functions

//    #region Event Functions
//    private void OnEnable()
//    {
//        GameSceneInitialiser.StartMainGame += StartMainGameTimer;
//    }

//    private void OnDisable()
//    {
//        GameSceneInitialiser.StartMainGame -= StartMainGameTimer;
//    }
//    #endregion
//    #region Update Function
//    private void Update()
//    {
//        if (!isTimerRunning) return;

//        //Update display timer
//        int displayTime = Mathf.CeilToInt(currentTime);
//        if (displayTime != lastWholeSecond)
//        {
//            lastWholeSecond = displayTime;
//            Debug.Log("Timer: " + displayTime);
//        }

//        //Internal logic timer
//        currentTime -= Time.deltaTime;

//        if (currentTime <= 0f)
//        {
//            currentTime = 0f;
//            isTimerRunning = false;

//            Debug.Log("Time's up!");
//        }
//    }
//    #endregion
//    #region Timer Functions
//    private void StartMainGameTimer(GameSceneInitialiser.GameRunStatus state)
//    {
//        //Only runs the rest of the function's code if enum value = GameStarted
//        if (state != GameSceneInitialiser.GameRunStatus.GameStarted)
//                return;

//        currentTime = mainGameTime;
//        isTimerRunning = true;
//    }
//    #endregion

//    #endregion
//}
