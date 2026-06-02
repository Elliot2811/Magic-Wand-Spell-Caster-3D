using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TempGameManager;

public class GameScenarioSelector : MonoBehaviour
{
    //Game Object References
    public TempGameManager mainGameManager;

    //Game Menu logic
    [Header("Game Menu Settings")]
    public int gameStartTimer = 5;
    private bool allowInputCheck = false;
    private bool leftCoinSlotInsert = false;
    private bool rightCoinSlotInsert = false;
    private Coroutine gameCountdownCoroutine;

    private void OnEnable()
    {
        TempGameManager.StartMainMenu += InputCheckSetTrue;
    }

    private void OnDisable()
    {
        TempGameManager.StartMainMenu -= InputCheckSetTrue;
    }

    private void Update()
    {
        if (allowInputCheck)
        {
            CoinInsertInput();
        }
    }

    public void InputCheckSetTrue()
    {
        allowInputCheck = true;
    }

    public void InputCheckSetFalse()
    {
        allowInputCheck = false;
    }

    private void CoinInsertInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            leftCoinSlotInsert = true;
            if (gameCountdownCoroutine == null)
            {
                gameCountdownCoroutine = StartCoroutine(GameStartCountdown());
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            rightCoinSlotInsert = true;
            if (gameCountdownCoroutine == null)
            {
                gameCountdownCoroutine = StartCoroutine(GameStartCountdown());
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && (gameCountdownCoroutine != null))
        {
            StopCoroutine(gameCountdownCoroutine);
            gameCountdownCoroutine = null;
            SelectScenario();
        }
    }

    private IEnumerator GameStartCountdown()
    {
        for (int i = gameStartTimer; i > 0; i--)
        {
            Debug.Log($"{i}!");
            yield return new WaitForSeconds(1);
        }
        SelectScenario();
    }

    private void SelectScenario()
    {
        if ((leftCoinSlotInsert == true) && (rightCoinSlotInsert == true))
        {
            mainGameManager.StartGame(3);
        }
        else if ((leftCoinSlotInsert == true) && (rightCoinSlotInsert == false))
        {
            mainGameManager.StartGame(1);
        }
        else if ((leftCoinSlotInsert == false) && (rightCoinSlotInsert == true))
        {
            mainGameManager.StartGame(2);
        }
        else
        {
            Debug.Log("Error -- Coin Insert if else statement logic is wrong");
        }
    }
}
