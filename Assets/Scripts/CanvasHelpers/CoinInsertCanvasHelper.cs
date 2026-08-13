using TMPro;
using UnityEngine;

public class CoinInsertCanvasHelper : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    [SerializeField] private TextMeshProUGUI transitionTimerText;
    [SerializeField] private TextMeshProUGUI transitionTimerOutline;

    [Space(10)]
    [SerializeField] private RectTransform leftCharacter;
    [SerializeField] private TextMeshProUGUI leftSlotStatus;
    [SerializeField] private RectTransform leftReadyPanel;

    [Space(10)]
    [SerializeField] private RectTransform rightCharacter;
    [SerializeField] private TextMeshProUGUI rightSlotStatus;
    [SerializeField] private RectTransform rightReadyPanel;

    private GameState currentState;

    private CoinInsertState coinInsertState;

    private void Start()
    {
        currentState = GameStateManager.Instance.CurrentState;

        if (currentState is not MapSelectionState)
        {
            Debug.LogError("[CoinInsertCanvasHelper]: Incorrect current state. Current state is not CoinInsertState");
            return;
        }

        coinInsertState = (CoinInsertState)currentState;
    }

    private void Update()
    {
        UpdateCoinStatus(coinInsertState.LeftCoin, coinInsertState.RightCoin);
        UpdateReadyStatus(coinInsertState.leftSideConfirmation, coinInsertState.rightSideConfirmation);
        UpdateCountdownText(coinInsertState.DisplayCountdown, coinInsertState.Countdown);
        UpdateTransitionTimerText(coinInsertState.DisplayTransitionTimer, coinInsertState.transitionTimer);
    }

    private void UpdateCoinStatus(bool leftCoin, bool rightCoin)
    {
        leftCharacter?.gameObject.SetActive(leftCoin);

        if (leftSlotStatus != null)
        {
            leftSlotStatus.text = leftCoin ? "1/1 Credits" : "0/1 Credits";
            leftSlotStatus.color = leftCoin ? Color.green : Color.white;
        }

        rightCharacter?.gameObject.SetActive(rightCoin);

        if (rightSlotStatus != null)
        {
            rightSlotStatus.text = rightCoin ? "1/1 Credits" : "0/1 Credits";
            rightSlotStatus.color = rightCoin ? Color.green : Color.white;
        }
    }

    private void UpdateReadyStatus(bool leftSideConfirmation,bool rightSideConfirmation)
    {
        leftReadyPanel?.gameObject.SetActive(leftSideConfirmation);
        rightReadyPanel?.gameObject.SetActive(rightSideConfirmation);
    }

    private void UpdateCountdownText(bool displayCountdown, float countdown)
    {
        if (countdownText == null) return;

        if (!displayCountdown)
        {
            countdownText.gameObject.SetActive(false);
            return;
        }

        countdownText.gameObject.SetActive(true);

        countdownText.text = Mathf.CeilToInt(countdown).ToString();
    }

    private void UpdateTransitionTimerText(bool displayTransitionTimer, float timer)
    {
        if (transitionTimerText == null && transitionTimerOutline == null) return;

        if (!displayTransitionTimer)
        {
            transitionTimerText?.gameObject.SetActive(false);

            transitionTimerOutline?.gameObject.SetActive(false);
            return;
        }

        string timerText = Mathf.CeilToInt(timer).ToString();

        if (transitionTimerText != null)
        {
            transitionTimerText.gameObject.SetActive(true);
            transitionTimerText.text = timerText;
        }

        if (transitionTimerOutline != null)
        {
            transitionTimerOutline.gameObject.SetActive(true);
            transitionTimerOutline.text = timerText;
        }
    }
}