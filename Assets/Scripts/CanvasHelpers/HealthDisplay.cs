//using System.Collections;
//using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    //    [Header("UI Healthbar")]
    //    public Slider HealthBar;

    //    [Header("State Machine Setup")]
    //    public VictoryState victoryState;

    //    [Header("Settings")]
    //    public float maxDamageAdvantage = 100f; //maximum amount of damage that player needs to have to immediately win the other player
    //    //e.g. if maxDamageAdvantage = 100f, then the player needs to deal 100 dmg more than the other player to win

    //    private CharacterEntity leftPlayer;
    //    private CharacterEntity rightPlayer;
    //    private bool gameEnded = false; //prevents triggering Victory state multiple times
    //    private bool isInitialized = false; //make sure player and map prefab is initialized

    //    void Start()
    //    {
    //        if (HealthBar != null)
    //        {
    //            HealthBar.minValue = 0f;
    //            HealthBar.maxValue = 1f;
    //            HealthBar.value = 0.5f;
    //        }
    //    }
    //    public void InitializePlayers(CharacterEntity left, CharacterEntity right)
    //    {
    //        leftPlayer = left;
    //        rightPlayer = right;
    //        isInitialized = true;
    //    }

    //    void Update()
    //    {
    //        if (!isInitialized || gameEnded || leftPlayer == null || rightPlayer == null || HealthBar == null)
    //            return;

    //        //Calculate the advantage: 
    //        //If Right Player takes damage, Left Player gets a positive advantage in healthbar
    //        //If Left Player takes damage, Right Player pushes back the healthbar
    //        float advantage = rightPlayer.EntityDmgTaken - leftPlayer.EntityDmgTaken;

    //        //If advantage is 0, value is 0.5 (Center of healthbar)
    //        //If left is winning, value moves towards 1 (Right side)
    //        float sliderTarget = 0.5f + (advantage / (maxDamageAdvantage * 2f));
    //        sliderTarget = Mathf.Clamp(sliderTarget, 0f, 1f);

    //        //Smoothly transition for the slider value so it slides nicely instead of snapping to the value
    //        HealthBar.value = Mathf.Lerp(HealthBar.value, sliderTarget, Time.deltaTime * 5f);

    //        // Win/Loss Checks
    //        if (sliderTarget >= 1f)
    //        {
    //            TriggerVictory(1); //Player 1 (left player) win
    //        }
    //        else if (sliderTarget <= 0f)
    //        {
    //            TriggerVictory(2); //Player 2 (right player) win
    //        }
    //    }
    //    private void TriggerVictory(int winnerID)
    //    {
    //        gameEnded = true;

    //        if (victoryState != null && GameStateManager.Instance != null)
    //        {
    //            //Tell the scriptable object who won so the victory scene can display it
    //            victoryState.SetWinner(winnerID);

    //            //Tell the state manager to switch to VictoryState
    //            GameStateManager.Instance.TransitionToState(victoryState);
    //        }
    //        else
    //        {
    //            Debug.LogError("Missing VictoryStateAsset on HealthDisplay or GameStateManager Instance is null!");
    //        }
    //    }

    private bool initialized = false;

    [SerializeField]
    private Slider HealthBar;

    [SerializeField]
    private TextMeshProUGUI countdownText;

    public float maxDamageAdvantage = 100f;
    public float lerpSpeed = 5f;

    private float displayedPercentage = 0.5f;

    private GamePlayState playState;

    private void Update()
    {
        if (!initialized)
        {
            initialized = SceneInitialized();
            if (!initialized)
                return;

            displayedPercentage = playState.displayPercentage;

            Debug.Log("Health Display Active");
        }

        UpdateHealthBar(playState.displayPercentage);
        UpdateTimer(playState.timer);

    }

    private bool SceneInitialized()
    {
        if (GameStateManager.Instance == null || GameStateManager.Instance.CurrentState is not GamePlayState)
            return false;

        playState = (GamePlayState)GameStateManager.Instance.CurrentState;

        return true;
    }

    private void UpdateHealthBar(float percentage)
    {
        if (HealthBar != null)
        {
            displayedPercentage = Mathf.Lerp(displayedPercentage, percentage, Time.deltaTime * lerpSpeed);
            HealthBar.value = displayedPercentage;
        }
    }

    private void UpdateTimer(float timer)
    {
        if (countdownText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);

            countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
