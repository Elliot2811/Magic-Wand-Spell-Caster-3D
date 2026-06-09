using TMPro;
using UnityEngine;

public class CoinInsertUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI leftSlotStatus;
    [SerializeField] private TextMeshProUGUI rightSlotStatus;

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
        UpdateCountdownText(coinInsertState.Countdown);
    }

    private void UpdateCoinStatus(bool leftCoin, bool rightCoin)
    {
        if (leftSlotStatus != null)
        {
            leftSlotStatus.text = leftCoin ? "P1 READY" : "INSERT COIN";
            leftSlotStatus.color = leftCoin ? Color.green : Color.white;
        }
        if (rightSlotStatus != null)
        {
            rightSlotStatus.text = rightCoin ? "P2 READY" : "INSERT COIN";
            rightSlotStatus.color = rightCoin ? Color.green : Color.white;
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