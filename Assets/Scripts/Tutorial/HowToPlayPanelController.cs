using System.Collections;
using UnityEngine;

public class HowToPlayPanelController : MonoBehaviour
{
    public static HowToPlayPanelController Instance { get; private set; }

    [SerializeField] private GameObject panel;           //How To Play Panel
    [SerializeField] private GameObject letsTryPanel;    //Let's Try Panel
    [SerializeField] private float instructionDuration = 4f;
    [SerializeField] private ShapeChecklistUI leftChecklist;
    [SerializeField] private ShapeChecklistUI rightChecklist;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(true);
        letsTryPanel.SetActive(false);
        StartCoroutine(ShowLetsTryAfterDelay());
    }

    private void OnDestroy() { if (Instance == this) Instance = null; }

    private IEnumerator ShowLetsTryAfterDelay()
    {
        yield return new WaitForSeconds(instructionDuration);
        panel.SetActive(false);
        letsTryPanel.SetActive(true);
    }

    public void Hide() => letsTryPanel.SetActive(false);

    public void MarkLeftDrawn(ShapeInfoSO shape)
    {
        Debug.Log($"MarkLeftDrawn called, leftChecklist={(leftChecklist != null ? "set" : "NULL")}");
        leftChecklist?.MarkDrawn(shape);
    }
    public void MarkRightDrawn(ShapeInfoSO shape)
    {
        Debug.Log($"MarkRightDrawn called, rightChecklist={(rightChecklist != null ? "set" : "NULL")}");
        rightChecklist?.MarkDrawn(shape);
    }
}