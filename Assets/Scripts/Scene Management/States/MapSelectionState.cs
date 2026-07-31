using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectionState : GameState
{
    [HideInInspector]
    public Rect map1Rect;
    [HideInInspector]
    public Rect map2Rect;

    private WandListener WandListenerLeft;
    private WandListener WandListenerRight;

    private int[] playerMapChoices = new int[] { -1, -1 };

    public bool transitioning { get; private set; } = false;
    public float timer { get; private set; } = 3f;
    private float timerDuration = 3f;

    protected override AudioPair Music => stateManager?.audioLibrary?.mapSelectionMusic;
    public override void EnterState(GameStateManager gameManager)
    {
        base.EnterState(gameManager);

        if (SceneManager.GetActiveScene().name != "MapSelection")
            SceneManager.LoadScene("MapSelection");

        WandListenerLeft = stateManager.wandListenerLeft;
        if (WandListenerLeft != null)
            WandListenerLeft.PointsForMatchedShape += FindLeftMapChosen;

        WandListenerRight = stateManager.wandListenerRight;
        if (WandListenerRight != null)
            WandListenerRight.PointsForMatchedShape += FindRightMapChosen;
    }

    public override void UpdateState()
    {
        if (!transitioning)
            return;

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            transitioning = false;
            stateManager.TransitionToState(GameStateManager.StateEnum.Fight);
        }
    }

    private void FindLeftMapChosen(Vector2[] points)
    {
        Vector2 centeroid = CompareShapes.Centroid(points);
        Vector2 screenCenteroid = Camera.main.WorldToScreenPoint(centeroid);

        int temp = GetMapIndexFromCenteroid(screenCenteroid);
        NewMapSelection?.Invoke(0, temp);
        playerMapChoices[0] = temp;

        Debug.Log("Player 1 chose map " + temp + ", centeroid: " + screenCenteroid);

        CheckMoveToNextMap();
    }

    private void FindRightMapChosen(Vector2[] points)
    {
        Vector2 centeroid = CompareShapes.Centroid(points);
        Vector2 screenCenteroid = Camera.main.WorldToScreenPoint(centeroid);

        int temp = GetMapIndexFromCenteroid(screenCenteroid);
        NewMapSelection?.Invoke(1, temp);
        playerMapChoices[1] = temp;

        Debug.Log("Player 2 chose map " + temp + ", centeroid: " + screenCenteroid);

        CheckMoveToNextMap();
    }

    private int GetMapIndexFromCenteroid(Vector2 centeroid)
    {
        if (map1Rect.Contains(centeroid))
            return 0;
        else if (map2Rect.Contains(centeroid))
            return 1;

        return -1;
    }

    private void CheckMoveToNextMap()
    {
        int mapChoice = playerMapChoices[0];

        if (mapChoice == -1)
        {
            transitioning = false;
            return;
        }

        for (int i = 1; i < playerMapChoices.Length; i++)
        {
            if (playerMapChoices[i] != mapChoice)
            {
                transitioning = false;
                return;
            }
        }

        stateManager.mapIndex = mapChoice;
        timer = timerDuration;
        transitioning = true;
    }

    public event Action<int, int> NewMapSelection;
}
