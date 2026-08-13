using System;
using System.Collections;
using UnityEngine;
using TMPro;
using static SpellBook;

[RequireComponent(typeof(RectTransform))]
public class MidGameEventBannerController : MonoBehaviour
{
    public static MidGameEventBannerController Instance { get; private set; }

    [SerializeField] private GameObject bannerPanel;
    [SerializeField] private TMP_Text bannerText;

    [Header("Timing")]
    [Tooltip("How long the banner sits fully on screen, not counting slide in/out.")]
    [SerializeField] private float bannerDuration = 3f;
    [SerializeField] private float slideInDuration = 0.8f;
    [SerializeField] private float slideOutDuration = 0.8f;

    [Header("Easing")]
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private RectTransform bannerRect;
    private Vector2 onScreenPos; //wherever you've placed the banner in the Inspector — treated as "centered/resting" position

    private void Awake()
    {
        Instance = this;

        if (bannerPanel != null)
        {
            bannerRect = bannerPanel.GetComponent<RectTransform>();
            onScreenPos = bannerRect.anchoredPosition;
            bannerPanel.SetActive(false);
        }
    }

    private void OnDestroy() { if (Instance == this) Instance = null; }

    //<summary>
    //Slides the banner in from off-screen left, holds for bannerDuration, then
    //slides out to off-screen right, then invokes onComplete. Callers (e.g.
    //GamePlayState) should wait for onComplete before starting the mid-game event.
    //</summary>
    public IEnumerator ShowBanner(string message, Action onComplete)
    {
        GameplayUIState.BlockSpellFeedback = true;
        Debug.Log("BLOCK ON: Title Panel");

        if (bannerPanel == null || bannerRect == null)
        {
            Debug.LogWarning("[MidGameEventBannerController]: no bannerPanel assigned — skipping banner.");
            onComplete?.Invoke();
            yield break;
        }

        if (bannerText != null)
            bannerText.text = message;

        //off-screen positions derived from the banner's own height now, instead of
        //width, since it's entering from the top and leaving out the bottom
        float offscreenDistance = ((RectTransform)bannerRect.parent).rect.height / 2f + bannerRect.rect.height / 2f;
        Vector2 topOffscreen = onScreenPos + Vector2.up * offscreenDistance;
        Vector2 bottomOffscreen = onScreenPos + Vector2.down * offscreenDistance;

        bannerPanel.SetActive(true);

        yield return SlidePosition(topOffscreen, onScreenPos, slideInDuration);

        yield return new WaitForSeconds(bannerDuration);

        yield return SlidePosition(onScreenPos, bottomOffscreen, slideOutDuration);

        bannerPanel.SetActive(false);
        bannerRect.anchoredPosition = onScreenPos; //reset for next time, since it's currently sitting off-screen bottom

        onComplete?.Invoke();

        GameplayUIState.BlockSpellFeedback = false;
        Debug.Log("BLOCK OFF: Title Panel");
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