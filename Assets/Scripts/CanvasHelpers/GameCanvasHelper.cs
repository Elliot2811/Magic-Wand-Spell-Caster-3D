//using System.Collections;
//using System.Collections.Generic;

using System.Linq;
using TMPro;
using UnityEngine;

public class GameCanvasHelper : MonoBehaviour
{
    private bool initialized = false;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private RectTransform LeftHealthBar;
    [SerializeField]
    private RectTransform RightHealthBar;
    private float healthBarWidth;
    private float displayedPercentage = 0.5f;

    [SerializeField]
    public DisplaySpellDrawing[] spellsLeft;
    [SerializeField]
    public DisplaySpellDrawing[] spellsRight;

    [SerializeField]
    private TextMeshProUGUI countdownText;

    public float lerpSpeed = 1f;
    private float lerpStart;
    private float lerpEnd;
    private float lerpTime;

    private GamePlayState playState;

    //private void Awake()
    //{
    //    Debug.Log($"[Awake] {gameObject.name} ({GetInstanceID()}) spellsLeft.Length = {spellsLeft?.Length ?? -1}");
    //}

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

        if (playState.redrawFlag)
        {
            playState.redrawFlag = false;

            //Debug.Log($"[{gameObject.name} / {GetInstanceID()}] spellsLeft.Length: {spellsLeft.Length}, spellsRight.Length: {spellsRight.Length}");
            UpdateDrawbleShapes(ref spellsLeft, playState.leftSpellCollection);
            UpdateDrawbleShapes(ref spellsRight, playState.rightSpellCollection);
        }
    }

    private bool SceneInitialized()
    {
        if (GameStateManager.Instance == null || GameStateManager.Instance.CurrentState is not GamePlayState)
            return false;

        playState = (GamePlayState)GameStateManager.Instance.CurrentState;
        if (playState == null)
        {
            Debug.LogError("No playstate object");
            gameObject.SetActive(false);
        }

        if (LeftHealthBar == null || RightHealthBar == null)
        {
            Debug.LogError("Either LeftHealthBar or RightHealthBar is not assigned");
            gameObject.SetActive (false);
        }

        healthBarWidth = LeftHealthBar.rect.width * 2;
        return true;
    }

    private void UpdateHealthBar(float percentage)
    {
        // TODO: Smoothly interpolate the health bar position based on the percentage

        if (Mathf.Approximately(displayedPercentage, percentage))
        {
            return;
        }

        if (lerpEnd != percentage)
        {
            lerpStart = displayedPercentage;
            lerpEnd = percentage;
            lerpTime = 0;
        }


        lerpTime += Time.deltaTime;
        float t = Mathf.Clamp01(lerpTime / lerpSpeed);

        displayedPercentage = Mathf.Lerp(lerpStart, lerpEnd, t);

        MoveHealthBar(displayedPercentage);
    }

    private void MoveHealthBar(float percentage)
    {
        float currLeftWidth = healthBarWidth * percentage + 1;

        LeftHealthBar.sizeDelta = new Vector2(currLeftWidth, LeftHealthBar.sizeDelta.y);

        float currRightWidth = healthBarWidth * (1 - percentage) + 1;
        RightHealthBar.sizeDelta = new Vector2(currRightWidth, RightHealthBar.sizeDelta.y);
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

    private void UpdateDrawbleShapes(ref DisplaySpellDrawing[] areas, ShapesCollectionSO collection)
    {
        if (areas == null || collection == null) return;

        ShapeInfoSO[] castableShapes = collection.GetAllShapes();

        int length = Mathf.Min(castableShapes.Length, areas.Length);

        for (int i = 0; i < length; i++)
        {
            if (areas[i] == null)
            {
                //Debug.LogWarning($"[{label}] Shape null");
                continue;
            }
            areas[i].RedrawIfNew(castableShapes[i]);
            //Debug.Log($"[{label}] Try redraw");
        }
    }
}
