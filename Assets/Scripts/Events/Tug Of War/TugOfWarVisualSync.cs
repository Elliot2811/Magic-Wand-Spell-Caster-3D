using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class TugOfWarVisualSync : MonoBehaviour
{
    [Header("References")]
    public Slider tugSlider;

    [Tooltip("The RectTransform that defines the full track width (usually Background, NOT Fill). " +
             "At value=0 the cluster sits at this rect's left edge; at value=1, its right edge.")]
    public RectTransform trackReference;

    private RectTransform selfRect;

    [Header("Optional")]
    [Tooltip("Shrinks the usable travel range slightly so the cluster's own width doesn't visually " +
             "overhang past the track edges at 0 or 1. Set to roughly half the cluster's pixel width.")]
    public float edgeInsetPixels = 0f;

    private void Awake()
    {
        selfRect = GetComponent<RectTransform>();

        if (tugSlider == null)
        {
            Debug.LogWarning("TugOfWarVisualSync: no Slider assigned, disabling.");
            enabled = false;
            return;
        }

        if (trackReference == null)
        {
            Debug.LogWarning("TugOfWarVisualSync: no trackReference assigned, disabling.");
            enabled = false;
            return;
        }

        // Pin anchors to a single point so this object is never stretched/resized, only repositioned.
        selfRect.anchorMin = new Vector2(0.5f, selfRect.anchorMin.y);
        selfRect.anchorMax = new Vector2(0.5f, selfRect.anchorMax.y);
    }

    private void OnEnable()
    {
        tugSlider.onValueChanged.AddListener(OnSliderValueChanged);
        // Apply correct position immediately in case value was set before this enabled (e.g. StartTugOfWar()).
        OnSliderValueChanged(tugSlider.value);
    }

    private void OnDisable()
    {
        if (tugSlider != null)
            tugSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        float trackWidth = trackReference.rect.width;
        float halfRange = (trackWidth / 2f) - edgeInsetPixels;

        // value=0 -> fully left (losing side for player2), value=1 -> fully right (losing side for player1)
        float xPos = Mathf.Lerp(-halfRange, halfRange, Mathf.Clamp01(value));

        selfRect.anchoredPosition = new Vector2(xPos, selfRect.anchoredPosition.y);
    }
}