using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Status Text")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private int playerIndex = 0; // set 0 for player1's checklist, 1 for player2's in the Inspector

    private HashSet<ShapeInfoSO> drawnShapes = new HashSet<ShapeInfoSO>();

    private void Awake()
    {
        foreach (var icon in icons)
            icon.image.color = undrawnColor;

        if (statusText != null)
            statusText.text = "";
    }

    private void OnEnable()
    {
        TutorialProgressTracker.OnBothPlayersDone += HandleBothPlayersDone;
    }

    private void OnDisable()
    {
        TutorialProgressTracker.OnBothPlayersDone -= HandleBothPlayersDone;
    }

    public void MarkDrawn(ShapeInfoSO shape)
    {
        if (shape == null) return;

        foreach (var icon in icons)
        {
            if (icon.shape == shape)
            {
                icon.image.color = drawnColor;
                drawnShapes.Add(shape);
                break;
            }
        }

        CheckIfChecklistComplete();
    }

    private void CheckIfChecklistComplete()
    {
        if (drawnShapes.Count >= icons.Length)
        {
            TutorialProgressTracker.ReportDone(playerIndex);

            if (statusText != null)
            {
                statusText.text = TutorialProgressTracker.BothDone
                    ? "Tutorial completed!"
                    : "Waiting for other player...";
            }
        }
    }

    private void HandleBothPlayersDone()
    {
        if (statusText != null)
            statusText.text = "Tutorial completed!";
    }
}