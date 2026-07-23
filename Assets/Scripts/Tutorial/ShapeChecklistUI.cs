using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeChecklistUI : MonoBehaviour
{
    [System.Serializable]
    public struct ShapeIcon
    {
        public ShapeInfoSO shape;
        public Image image;
    }

    [SerializeField] private ShapeIcon[] icons;
    [SerializeField] private Color undrawnColor = new Color(0.35f, 0.35f, 0.35f);
    [SerializeField] private Color drawnColor = Color.white;

    private void Awake()
    {
        foreach (var icon in icons)
            icon.image.color = undrawnColor;
    }

    public void MarkDrawn(ShapeInfoSO shape)
    {
        Debug.Log($"[ShapeChecklistUI] MarkDrawn entered on {gameObject.name} with shape={shape?.ShapeName ?? "null"}, icons.Length={icons.Length}");
        if (shape == null) return;

        foreach (var icon in icons)
        {
            Debug.Log($"  comparing against icon slot: {icon.shape?.ShapeName ?? "null"}, match={icon.shape == shape}");
            if (icon.shape == shape)
            {
                icon.image.color = drawnColor;
                Debug.Log($"  -> color set to drawnColor");
                break;
            }
        }
    }
}