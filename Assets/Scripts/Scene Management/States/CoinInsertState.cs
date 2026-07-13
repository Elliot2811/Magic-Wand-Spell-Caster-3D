using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CoinInsertState : GameState
{
    public float Countdown { get; private set; }
    public bool DisplayCountdown { get; private set; } = false;

    private float transitionTime = 6f;
    public float transitionTimer { get; private set; } = 6f;
    public bool DisplayTransitionTimer { get; private set; } = false;

    public bool LeftCoin { get; private set; } = false;
    public bool leftSideConfirmation = false;

    public bool RightCoin { get; private set; } = false;
    public bool rightSideConfirmation = false;

    protected override AudioPair Music => stateManager?.audioLibrary?.coinInsertMusic;

    #region Private values to reduce stateManager queries
    private Wand wandLeft;
    private Wand wandRight;

    private WandListener wandListenerLeft;
    private WandListener wandListenerRight;
    #endregion

    CoinInsertListener coinInsertListener = null;

    public override void EnterState(GameStateManager gameManager)
    {
        base.EnterState(gameManager);

        if (SceneManager.GetActiveScene().name != "CoinInsert")
            SceneManager.LoadScene("CoinInsert");

        Countdown = GameConstants.coinInsertionCountdownTime;
        coinInsertListener = new CoinInsertListener();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (!LeftCoin && coinInsertListener.LeftCoinInsertion())
        {
            LeftCoin = true;
            stateManager.gameScenario += 0b_1;

            Countdown = GameConstants.coinInsertionCountdownTime;
            DisplayCountdown = true;
            transitionTimer = transitionTime;
            DisplayTransitionTimer = false;

            wandLeft = MonoBehaviour.Instantiate(stateManager.wandPrefab);
            MonoBehaviour.DontDestroyOnLoad(wandLeft);
            wandLeft.Init(true, 0);
            stateManager.wandLeft = wandLeft;


            wandListenerLeft = MonoBehaviour.Instantiate(stateManager.wandListenerPrefab, wandLeft.transform);
            wandListenerLeft.Init(wandLeft, shapes: stateManager.allShapesCollectionSO);
            wandListenerLeft.MatchedShape += CheckLeftDrawing;
            stateManager.wandListenerLeft = wandListenerLeft;

            Debug.Log("Left wand has been created");
        }

        if (!RightCoin && coinInsertListener.rightCoinInsertion())
        {
            RightCoin = true;
            stateManager.gameScenario += 0b_10;
            
            Countdown = GameConstants.coinInsertionCountdownTime;
            DisplayCountdown = true;
            transitionTime = transitionTimer;
            DisplayTransitionTimer = false;

            wandRight = MonoBehaviour.Instantiate(stateManager.wandPrefab);
            MonoBehaviour.DontDestroyOnLoad(wandRight);
            wandRight.Init(true, 1);
            stateManager.wandRight = wandRight;


            wandListenerRight = MonoBehaviour.Instantiate(stateManager.wandListenerPrefab, wandRight.transform);
            wandListenerRight.Init(wandRight, shapes: stateManager.allShapesCollectionSO);
            wandListenerRight.MatchedShape += CheckRightDrawing;
            stateManager.wandListenerRight = wandListenerRight;
            Debug.Log("Right wand has been created");

        }

        if ((LeftCoin && RightCoin))
        {
            //if (leftSideConfirmation && rightSideConfirmation)
            //{
            //    TransitionToFightState();
            //    DisplayCountdown = false;
            //}
            //else
            //    Countdown -= Time.deltaTime;

            if (leftSideConfirmation && rightSideConfirmation)
            {
                TransitionToFightState();
                DisplayCountdown = false;
            }
            else if (Countdown > 0f)
            {
                Countdown -= Time.deltaTime;
            }
            else
            {
                LeftCoin = false;
                RightCoin = false;
                coinInsertListener.Reset();
                Countdown = GameConstants.coinInsertionCountdownTime;
                GameStateManager.Instance.ResetGameAndWand();
                DisplayCountdown = false;
                DisplayTransitionTimer = false;
                leftSideConfirmation = false;
                rightSideConfirmation = false;
            }
        }

        if (LeftCoin ^ RightCoin)
        {
            if ((LeftCoin && leftSideConfirmation && !RightCoin) || (RightCoin && rightSideConfirmation && !LeftCoin))
            {
                TransitionToFightState();
                DisplayCountdown = false;
                return;
            }
            else if (Countdown > 0f)
            {
                Countdown -= Time.deltaTime;
            }
            else
            {
                LeftCoin = false;
                RightCoin = false;
                coinInsertListener.Reset();
                Countdown = GameConstants.coinInsertionCountdownTime;
                GameStateManager.Instance.ResetGameAndWand();
                DisplayCountdown = false;
                DisplayTransitionTimer = false;
                leftSideConfirmation = false;
                rightSideConfirmation = false;
            }
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
            rightSideConfirmation = true;
    }

    private void TransitionToFightState()
    {
        transitionTimer -= Time.deltaTime;
        DisplayTransitionTimer = true;

        if (transitionTimer <= 0)
        {
            stateManager.TransitionToState(GameStateManager.StateEnum.Fight);
        }
    }
}

public class CoinInsertListener
{
    private bool leftCoin;
    private bool rightCoin;

    private JoyConTracker controllerInstance;
    private WandInputActions wandInputActions;

    public CoinInsertListener()
    {
        leftCoin = false;
        rightCoin = false;

        controllerInstance = JoyConTracker.Instance;

        if (controllerInstance != null && controllerInstance.gameObject.activeInHierarchy)
            controllerInstance.drawingButtonPressed += HandleJoyConButtonPress;
        else
        {
            wandInputActions = new WandInputActions();
            wandInputActions.Enable();
            wandInputActions.Wand.DrawingLeft.started += HandleLeftButtonPress;
            wandInputActions.Wand.DrawingRight.started += HandleRightButtonPress;

        }

    }

    ~CoinInsertListener()
    {
        if (controllerInstance != null && controllerInstance.gameObject.activeInHierarchy)
            controllerInstance.drawingButtonPressed -= HandleJoyConButtonPress;
        else if (wandInputActions != null)
        {
            wandInputActions.Wand.DrawingLeft.started -= HandleLeftButtonPress;
            wandInputActions.Wand.DrawingRight.started -= HandleRightButtonPress;
            wandInputActions.Disable();
        }
    }

    private void HandleJoyConButtonPress(int handle)
    {
        if (handle == 0)
            leftCoin = true;

        if (handle == 1)
            rightCoin = true;
    }

    private void HandleLeftButtonPress(InputAction.CallbackContext context) 
    {
        leftCoin = true;
    }

    private void HandleRightButtonPress(InputAction.CallbackContext context)
    {
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