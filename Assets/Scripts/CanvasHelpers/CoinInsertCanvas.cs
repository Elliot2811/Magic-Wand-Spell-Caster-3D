using TMPro;
using UnityEngine;

public class CoinInsertUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI leftSlotStatus;
    [SerializeField] private TextMeshProUGUI leftReadyStatus;
    [SerializeField] private TextMeshProUGUI rightSlotStatus;
    [SerializeField] private TextMeshProUGUI rightReadyStatus;

    private GameState currentState;

    private CoinInsertState coinInsertState;

    private void Start()
    {
        currentState = GameStateManager.Instance.CurrentState;

        if (currentState is not CoinInsertState)
            Debug.LogError("[CoinInsertCanvas]: Incorrect current state. Current state is not CoinInsertState");

        coinInsertState = (CoinInsertState)currentState;
    }

    private void Update()
    {
        UpdateCoinStatus(coinInsertState.LeftCoin, coinInsertState.RightCoin);
        UpdateReadyStatus(
            coinInsertState.LeftCoin,
            coinInsertState.leftSideConfirmation,
            coinInsertState.RightCoin,
            coinInsertState.RightSideConfirmation
            );
        UpdateCountdownText(coinInsertState.Countdown);
    }

    private void UpdateCoinStatus(bool leftCoin, bool rightCoin)
    {
        if (leftSlotStatus != null)
        {
            leftSlotStatus.text = leftCoin ? "P1 COIN INESERTED" : "INSERT COIN";
            leftSlotStatus.color = leftCoin ? Color.green : Color.white;
        }
        if (rightSlotStatus != null)
        {
            rightSlotStatus.text = rightCoin ? "P2 COIN INSERTED" : "INSERT COIN";
            rightSlotStatus.color = rightCoin ? Color.green : Color.white;
        }
    }

    private void UpdateReadyStatus(bool leftCoin, bool leftSideConfirmation, bool rightCoin, bool rightSideConfirmation)
    {
        if (leftReadyStatus != null)
        {
            if (leftCoin)
            {
                leftReadyStatus.text = leftSideConfirmation ? "P1 Ready" : "Ready up?";
                leftReadyStatus.color = leftSideConfirmation ? Color.green : Color.white;
            }
            else
                leftReadyStatus.text = "";
        }

        if (rightReadyStatus != null)
        {
            if (rightCoin)
            {
                rightReadyStatus.text = rightSideConfirmation ? "P2 Ready" : "Ready Up?";
                rightReadyStatus.color = rightSideConfirmation ? Color.green : Color.white;
            }
            else
                rightReadyStatus.text = "";
        }
    }

    private void UpdateCountdownText(float countdown)
    {
        if (countdownText == null) return;

        bool countdownActive = countdown < GameConstants.coinInsertionCountdownTime
                               && countdown > 0;
        countdownText.gameObject.SetActive(countdownActive);

        if (countdownActive)
            countdownText.text = Mathf.CeilToInt(countdown).ToString();

    }
}