using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanelController : MonoBehaviour
{
    public static TutorialPanelController Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private float hideDelay = 2f;

    private bool hiding = false;

    private void Awake() => Instance = this;
    private void OnDestroy() { if (Instance == this) Instance = null; }

    public void HideAfterDelay()
    {
        if (hiding || panel == null || !panel.activeSelf) return;
        hiding = true;
        StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        yield return new WaitForSeconds(hideDelay);
        panel.SetActive(false);
    }
}