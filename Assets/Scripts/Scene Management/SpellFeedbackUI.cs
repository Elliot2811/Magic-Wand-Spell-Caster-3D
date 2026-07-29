using System.Collections;
using TMPro;
using UnityEngine;

public class SpellFeedbackUI : MonoBehaviour
{
    public static SpellFeedbackUI Instance;

    [SerializeField] private TMP_Text leftFeedbackText;
    [SerializeField] private TMP_Text rightFeedbackText;

    [SerializeField] private float displayTime = 2f;

    private Coroutine leftRoutine;
    private Coroutine rightRoutine;

    private void Awake()
    {
        Instance = this;

        leftFeedbackText.text = "";
        rightFeedbackText.text = "";

        leftFeedbackText.enabled = false;
        rightFeedbackText.enabled = false;
    }

    public void ShowFeedback(int playerNumber, string text)
    {
        if (playerNumber == 0)
        {
            if (leftRoutine != null)
                StopCoroutine(leftRoutine);

            leftRoutine = StartCoroutine(DisplayRoutine(leftFeedbackText, text, true));
        }
        else
        {
            if (rightRoutine != null)
                StopCoroutine(rightRoutine);

            rightRoutine = StartCoroutine(DisplayRoutine(rightFeedbackText, text, false));
        }
    }

    private IEnumerator DisplayRoutine(TMP_Text target, string text, bool isLeft)
    {
        target.text = text;
        target.enabled = true;

        yield return new WaitForSeconds(displayTime);

        target.text = "";
        target.enabled = false;

        if (isLeft)
            leftRoutine = null;
        else
            rightRoutine = null;
    }
}