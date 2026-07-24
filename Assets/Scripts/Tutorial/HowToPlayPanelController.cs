using System.Collections;
using UnityEngine;

public class HowToPlayPanelController : MonoBehaviour
{
    public static HowToPlayPanelController Instance { get; private set; }
    [SerializeField] private GameObject panel;           //How To Play Panel
    [SerializeField] private GameObject letsTryPanel;    //Let's Try Panel
    [SerializeField] private float recapDuration = 4f;   //renamed from instructionDuration — now how long the RECAP shows, not the upfront instructions
    [SerializeField] private ShapeChecklistUI leftChecklist;
    [SerializeField] private ShapeChecklistUI rightChecklist;

    private void Awake()
    {
        Instance = this;

        //Let's Try comes first now — players jump straight into drawing.
        //How To Play is shown afterward as a brief recap instead of upfront.
        panel.SetActive(false);
        letsTryPanel.SetActive(true);
    }

    private void OnDestroy() { if (Instance == this) Instance = null; }

    //<summary>
    //Called once both players have finished drawing all required shapes
    //(GamePlayState's WaitForHowToPlayConfirmation). Hides Let's Try, shows the
    //How To Play panel briefly as a recap, then hides it too. GamePlayState
    //should yield on this directly so the match doesn't start until the recap's done.
    //</summary>
    public IEnumerator ShowRecapThenHide()
    {
        letsTryPanel.SetActive(false);
        panel.SetActive(true);

        yield return new WaitForSeconds(recapDuration);

        panel.SetActive(false);
    }

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