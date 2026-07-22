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

    //Call this from the WandListener.MatchedShape callback for this side
    public void MarkDrawn(ShapeInfoSO shape)
    {
        if (shape == null) return;

        foreach (var icon in icons)
        {
            if (icon.shape == shape)
            {
                icon.image.color = drawnColor;
                break;
            }
        }
    }
}