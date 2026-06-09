using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinInsertState : GameState
{
    public float Countdown { get; private set; }

    public bool LeftCoin = false;
    public bool RightCoin = false;

    public override void EnterState(GameStateManager gameManager)
    {
        base.EnterState(gameManager);

        //Load Main Menu Screen
        if (SceneManager.GetActiveScene().name != "CoinInsert")
        {
            SceneManager.LoadScene("CoinInsert");
        }

        Countdown = GameConstants.coinInsertionCountdownTime;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (!LeftCoin && CoinInsertHelper.LeftCoinInsertion())
        {
            LeftCoin = true;
            stateManager.gameScenario += 0b_1;
        }

        if (!RightCoin && CoinInsertHelper.rightCoinInsertion())
        {
            RightCoin = true;
            stateManager.gameScenario += 0b_10;
        }

        if (LeftCoin || RightCoin)
        {
            Countdown -= Time.deltaTime;
        }

        if ((LeftCoin && RightCoin) || Countdown < 0)
        {
            stateManager.TransitionToState(GameStateManager.StateEnum.MapSelect);
            return;
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}

public static class CoinInsertHelper
{
    public static bool LeftCoinInsertion()
    {
        return (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W));
    }

    public static bool rightCoinInsertion()
    {
        return (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.I));
    }
}