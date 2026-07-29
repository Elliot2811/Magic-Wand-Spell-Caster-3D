using System.Collections;
using UnityEngine;
using TMPro;

public class CountdownController : MonoBehaviour
{
    public static CountdownController Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text countdownText;

    [Header("Timing")]
    [SerializeField] private float secondsPerNumber = 1f;
    [SerializeField] private string goText = "GO!";
    [SerializeField] private float goDuration = 0.5f;

    private void Awake()
    {
        Instance = this;
        if (panel != null)
            panel.SetActive(false);
    }

    private void OnDestroy() { if (Instance == this) Instance = null; }

    //<summary>
    //Shows 3, 2, 1, GO! in sequence. Callers should yield on this before
    //letting gameplay actually start (timer, spell casting, etc).
    //</summary>
    public IEnumerator ShowCountdown(int from = 3)
    {
        if (panel == null || countdownText == null)
        {
            Debug.LogWarning("[CountdownController]: not assigned — skipping countdown.");
            yield break;
        }

        panel.SetActive(true);

        for (int i = from; i >= 1; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(secondsPerNumber);
        }

        countdownText.text = goText;
        yield return new WaitForSeconds(goDuration);

        panel.SetActive(false);
    }
}