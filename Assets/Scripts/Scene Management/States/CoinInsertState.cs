using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinInsertState : GameState
{
    public float Countdown { get; private set; }

    private bool coroutineStarted = false;

    public bool LeftCoin { get; private set; } = false;
    public bool RightCoin { get; private set; } = false;
    public bool leftSideConfirmation = false;
    public bool RightSideConfirmation = false;

    protected override AudioClip Music => stateManager?.audioLibrary?.coinInsertMusic;

    #region Private values to reduce stateManager queries
    private Wand wandLeft;
    private Wand wandRight;

    private WandListener wandListenerLeft;
    private WandListener wandListenerRight;
    #endregion

    CoinInsertHelper helperFunc = null;

    public override void EnterState(GameStateManager gameManager)
    {
        base.EnterState(gameManager);

        if (SceneManager.GetActiveScene().name != "CoinInsert")
            SceneManager.LoadScene("CoinInsert");

        Countdown = GameConstants.coinInsertionCountdownTime;
        helperFunc = new CoinInsertHelper();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (!LeftCoin && helperFunc.LeftCoinInsertion())
        {
            LeftCoin = true;
            stateManager.gameScenario += 0b_1;

            wandLeft = MonoBehaviour.Instantiate(stateManager.wandPrefab);
            MonoBehaviour.DontDestroyOnLoad(wandLeft);
            wandLeft.Init(true, 0);
            stateManager.wandLeft = wandLeft;


            wandListenerLeft = MonoBehaviour.Instantiate(stateManager.wandListenerPrefab, wandLeft.transform);
            wandListenerLeft.Init(wandLeft, shapes: stateManager.allShapesCollectionSO);
            wandListenerLeft.MatchedShape += CheckLeftDrawing;
            stateManager.wandListenerLeft = wandListenerLeft;
        }

        if (!RightCoin && helperFunc.rightCoinInsertion())
        {
            RightCoin = true;
            stateManager.gameScenario += 0b_10;

            wandRight = MonoBehaviour.Instantiate(stateManager.wandPrefab);
            MonoBehaviour.DontDestroyOnLoad(wandRight);
            wandRight.Init(true, 1);
            stateManager.wandRight = wandRight;


            wandListenerRight = MonoBehaviour.Instantiate(stateManager.wandListenerPrefab, wandRight.transform);
            wandListenerRight.Init(wandRight, shapes: stateManager.allShapesCollectionSO);
            wandListenerRight.MatchedShape += CheckRightDrawing;
            stateManager.wandListenerRight = wandListenerRight;
        }

        if ((LeftCoin && RightCoin))
            if (leftSideConfirmation && RightSideConfirmation)
                if (!coroutineStarted)
                {
                    coroutineStarted = true;
                    stateManager.TransitionToState(GameStateManager.StateEnum.Fight, 2f);
                }

        if (LeftCoin ^ RightCoin)
        {
            if ((LeftCoin && leftSideConfirmation) || (RightCoin && RightSideConfirmation))
                if (!coroutineStarted)
                {
                    coroutineStarted = true;
                    stateManager.TransitionToState(GameStateManager.StateEnum.Fight, 2f);
                }

            Countdown -= Time.deltaTime;
        }

        // Implement resetng and dispensing coins back to player.
        //if (Countdown <= 0)
        //{
        //    Reset();
        //}
    }

    public override void ExitState()
    {
        base.ExitState();
    }


    // TODO: FIX this
    //private void Reset()
    //{
    //    stateManager.ResetWands();

    //    LeftCoin = false;
    //    RightCoin = false;

    //    Countdown = GameConstants.coinInsertionCountdownTime;

    //    helperFunc.Reset();
    //}

    private void CheckLeftDrawing(ShapeInfoSO shapeInfo)
    {
        if (shapeInfo != null)
            leftSideConfirmation = true;
    }

    private void CheckRightDrawing(ShapeInfoSO shapeInfo)
    {
        if (shapeInfo != null)
            RightSideConfirmation = true;
    }
}

public class CoinInsertHelper
{
    private bool leftCoin;
    private bool rightCoin;

    public CoinInsertHelper()
    {
        leftCoin = false;
        rightCoin = false;

        JoyConTracker.Instance.drawingButtonPressed += HandleJoyConButtonPress;
    }

    ~CoinInsertHelper()
    {
        JoyConTracker.Instance.drawingButtonPressed -= HandleJoyConButtonPress;
    }

    private void HandleJoyConButtonPress(int handle)
    {
        if (handle == 0)
            leftCoin = true;

        if (handle == 1)
            rightCoin = true;
    }

    public bool LeftCoinInsertion()
    {
        //return (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W));
        return leftCoin;
    }

    public bool rightCoinInsertion()
    {
        //return (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.I));
        return rightCoin;
    }

    public void Reset()
    {
        leftCoin = false;
        rightCoin = false;
    }
}