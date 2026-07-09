//using System.Collections;
//using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
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
