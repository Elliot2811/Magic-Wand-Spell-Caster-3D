using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlayPanelController : MonoBehaviour
{
    public static HowToPlayPanelController Instance { get; private set; }
    [SerializeField] private GameObject panel;

    private void Awake() => Instance = this;
    private void OnDestroy() { if (Instance == this) Instance = null; }

    public void Hide() => panel.SetActive(false);
}