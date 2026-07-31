using System;
using System.Collections;
using UnityEngine;
using TMPro;
using static SpellBook;

public class MidGameEventInstructionController : MonoBehaviour
{
    public static MidGameEventInstructionController Instance { get; private set; }
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text instructionText;

    [Header("Timing")]
    [Tooltip("How long the panel sits fully on screen, not counting slide in/out.")]
    [SerializeField] private float bannerDuration = 4f;
    [SerializeField] private float slideInDuration = 0.8f;
    [SerializeField] private float slideOutDuration = 0.8f;

    [Header("Easing")]
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private RectTransform bannerRect;
    private Vector2 onScreenPos; //wherever you've placed the panel in the Inspector — treated as "centered/resting" position

    private void Awake()
    {
        Instance = this;
        if (instructionPanel != null)
        {
            bannerRect = instructionPanel.GetComponent<RectTransform>();
            onScreenPos = bannerRect.anchoredPosition;
            instructionPanel.SetActive(false);
        }
    }

    private void OnDestroy() { if (Instance == this) Instance = null; }

    //<summary>
    //Slides the panel in from off-screen top, shows the given event's title +
    //instructions, holds for bannerDuration, then slides out to off-screen
    //bottom. Callers should yield on this before actually starting the
    //mid-game event.
    //</summary>
    public IEnumerator ShowInstructions(string title, string message)
    {
        GameplayUIState.BlockSpellFeedback = true;
        Debug.Log("BLOCK ON: Instruction Panel");

        if (instructionPanel == null || bannerRect == null)
        {
            Debug.LogWarning("[MidGameEventInstructionController]: no instructionPanel assigned — skipping.");
            yield break;
        }

        if (titleText != null)
            titleText.text = title;
        if (instructionText != null)
            instructionText.text = message;

        //off-screen positions derived from the panel's own height, so this
        //works regardless of screen resolution/aspect ratio without hand-tuning pixels
        float offscreenDistance = ((RectTransform)bannerRect.parent).rect.height / 2f + bannerRect.rect.height / 2f;
        Vector2 topOffscreen = onScreenPos + Vector2.up * offscreenDistance;
        Vector2 bottomOffscreen = onScreenPos + Vector2.down * offscreenDistance;

        instructionPanel.SetActive(true);

        yield return SlidePosition(topOffscreen, onScreenPos, slideInDuration);

        yield return new WaitForSeconds(bannerDuration);

        yield return SlidePosition(onScreenPos, bottomOffscreen, slideOutDuration);

        instructionPanel.SetActive(false);
        bannerRect.anchoredPosition = onScreenPos; //reset for next time, since it's currently sitting off-screen bottom
        
        GameplayUIState.BlockSpellFeedback = false;
        Debug.Log("BLOCK OFF: Instruction Panel");
    }

    private IEnumerator SlidePosition(Vector2 from, Vector2 to, float duration)
    {
        if (duration <= 0f)
        {
            bannerRect.anchoredPosition = to;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = slideCurve.Evaluate(Mathf.Clamp01(elapsed / duration));
            bannerRect.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

        bannerRect.anchoredPosition = to;
    }
}