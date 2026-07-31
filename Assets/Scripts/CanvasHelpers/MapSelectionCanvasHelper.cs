using System;
using TMPro;
using UnityEngine;

public class MapSelectionCanvasHelper : MonoBehaviour
{
    [Serializable]
    public class MapSelectionGameObjectsCollection
    {
        public GameObject[] gameObjects;
    }

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private RectTransform map1Image;
    [SerializeField]
    private RectTransform map2Image;

    [SerializeField]
    private TextMeshProUGUI transitionTimerText;
    [SerializeField]
    private TextMeshProUGUI transitionTimerOutline;

    [SerializeField]
    private MapSelectionGameObjectsCollection[] gameObjectsCollection;

    private MapSelectionState state;

    private void Start()
    {
        GameState currentState = GameStateManager.Instance.CurrentState;

        if (currentState is not MapSelectionState)
        {
            Debug.LogError("[CoinInsertCanvas]: Incorrect current state. Current state is not MainMenuState");
            return;
        }

        state = (MapSelectionState)currentState;

        state.NewMapSelection += ActivateMapSelection;

        state.map1Rect = FindRect(map1Image);
        state.map2Rect = FindRect(map2Image);

        foreach (MapSelectionGameObjectsCollection i in  gameObjectsCollection)
        {
            foreach (GameObject j in i.gameObjects)
            {
                j.SetActive(false);
            }
        }
    }

    private void Update()
    {
        UpdateTransitionTimerText(state.transitioning, state.timer);
    }

    private Rect FindRect(RectTransform rectTransform)
    {
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);

        Vector2 min = RectTransformUtility.WorldToScreenPoint(cam, worldCorners[0]);
        Vector2 max = RectTransformUtility.WorldToScreenPoint(cam, worldCorners[2]);

        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }

    private void ActivateMapSelection(int playerIndex, int mapIndex)
    {
        if (playerIndex < 0 || playerIndex >= gameObjectsCollection.Length)
            return;

        if (gameObjectsCollection[playerIndex] == null)
        {
            Debug.LogError("PlayerIndex exceeds created popups for map selection");
            return;
        }

        GameObject[] i = gameObjectsCollection[playerIndex].gameObjects;
        
        for (int j = 0; j < i.Length; j++)
        {
            if (j != mapIndex)
                i[j].SetActive(false);
            else
                i[j].SetActive(true);
        }
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

    private void OnDestroy()
    {
        if (state != null)
            state.NewMapSelection -= ActivateMapSelection;
    }
}
