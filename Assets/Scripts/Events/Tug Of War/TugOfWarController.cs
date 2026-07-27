using System;
using UnityEngine;
using UnityEngine.UI;

//<summary>
//Mid-match tug-of-war minigame.
//Player 1 (left) shakes to push the slider RIGHT (toward value 1).
//Player 2 (right) shakes to push the slider LEFT (toward value 0).
//Whoever pushes the bar all the way to the other side wins.
//</summary>
public class TugOfWarController : MonoBehaviour, IMidGameEvent
{
    [Header("UI References")]
    public GameObject panel;
    public Slider tugSlider;

    [Header("Player device indices")] //JoyCon controller index, typically 0 = left, 1 = right
    public int player1Index = 0;  //left player's JoyCon device index
    public int player2Index = 1;  //right player's JoyCon device index

    [Header("Tuning")]
    [Tooltip("How strongly normalized cursor-movement difference moves the slider per second. LOWERED from 0.5 so a match takes a real back-and-forth instead of one shake burst deciding it.")]
    public float pushPower = 0.3f;

    [Tooltip("How fast the accumulated shake energy decays per second when the player stops shaking. Slightly raised so lulls in shaking actually cost you momentum.")]
    public float shakeDecayRate = 5f;

    [Tooltip("Cursor movement below this (as a fraction of screen size, ~0-1) is treated as noise/drift and ignored.")]
    public float shakeNoiseFloor = 0.005f;

    [Tooltip("Hard cap on a single player's accumulated shake intensity. Prevents one big shake spike from swinging (or ending) the match instantly - forces sustained shaking to keep an advantage.")]
    public float maxShakeIntensity = 0.5f;

    [Tooltip("How much harder the bar is to push the closer it gets to a win (0 = no extra resistance at the edges, 1 = fully stuck). Creates the 'last stretch is the hardest' feel instead of a flat slide to victory.")]
    [Range(0f, 0.95f)]
    public float edgeResistanceFactor = 0.55f;

    [Tooltip("Optional: auto-end the event after this many seconds if nobody wins. 0 = no limit for testing.")]
    public float maxDuration = 0f;

    public string EventTitle => "TUG OF WAR!";
    public string EventInstructions => "SHAKE YOUR CONTROLLER!";

    public bool IsActive { get; private set; }
    public bool PausesMainTimer => true;

    private float elapsed;
    private JoyConTracker tracker;

    //Fired with the winning player's number: 1 or 2 (NOT the device index)
    public event Action<int> OnTugOfWarWon;
    public event Action OnTugOfWarTimedOut;

    //IMidGameEvent: fires once with 0 (timeout/draw), 1 (left won) or 2 (right won)
    public event Action<int> OnEventCompleted;

    [Header("Testing")]
    public bool autoStartForTesting = false;

    //Per-device NORMALIZED (0-1) screen-space shake tracking
    private Vector2[] prevViewportPos = new Vector2[16];
    private float[] shakeIntensity = new float[16];

    private void Awake()
    {
        tracker = JoyConTracker.Instance;

        if (panel != null)
            panel.SetActive(false);
    }

    private void Start()
    {
        Debug.Log($"TugOfWarController: Start() called, autoStartForTesting={autoStartForTesting}");

        if (autoStartForTesting)
            StartCoroutine(AutoStartWhenReady());
    }

    private System.Collections.IEnumerator AutoStartWhenReady()
    {
        Debug.Log("TugOfWarController: waiting for JoyConTracker.readyToConnect...");

        while (tracker == null || !tracker.readyToConnect)
        {
            tracker = JoyConTracker.Instance;
            yield return null;
        }

        Debug.Log("TugOfWarController: tracker ready, starting event.");
        StartTugOfWar();
    }

    //<summary>IMidGameEvent entry point — just forwards to the existing start method.</summary>
    public void StartEvent()
    {
        StartTugOfWar();
    }

    public void StartTugOfWar()
    {
        if (tracker == null)
            tracker = JoyConTracker.Instance;

        if (tracker == null)
        {
            Debug.LogWarning("TugOfWarController: no JoyConTracker instance found.");
            return;
        }

        if (player1Index == player2Index)
        {
            Debug.LogError($"TugOfWarController: player1Index and player2Index are both {player1Index}! " +
                            "Set them to different device indices (e.g. 0 and 1) in the Inspector.");
            return;
        }

        if (!tracker.readyToConnect)
        {
            Debug.LogWarning("TugOfWarController: StartTugOfWar() called before tracker.readyToConnect is true. " +
                              "Gyro reads will return zero until it's ready.");
        }

        if (tugSlider == null)
        {
            Debug.LogError("TugOfWarController: tugSlider is not assigned in the Inspector.");
            return;
        }

        shakeIntensity[player1Index] = 0f;
        shakeIntensity[player2Index] = 0f;
        prevViewportPos[player1Index] = GetNormalizedPos(player1Index);
        prevViewportPos[player2Index] = GetNormalizedPos(player2Index);

        tugSlider.value = 0.5f;

        if (panel != null)
            panel.SetActive(true);

        elapsed = 0f;
        IsActive = true;

        Debug.Log($"TugOfWarController: started. P1 idx={player1Index}, P2 idx={player2Index}, tracker ready={tracker.readyToConnect}");
    }

    public void StopTugOfWar()
    {
        IsActive = false;

        if (panel != null)
            panel.SetActive(false);
    }

    private void Update()
    {
        if (!IsActive || tracker == null || tugSlider == null)
            return;

        elapsed += Time.deltaTime;

        float p1Shake = UpdateAndGetShake(player1Index);
        float p2Shake = UpdateAndGetShake(player2Index);

        float diff = p1Shake - p2Shake;

        // Edge resistance: the further the bar is from center, the harder it is to keep pushing
        // in that same direction. distanceFromCenter is 0 at the middle, 1 at either edge.
        float distanceFromCenter = Mathf.Abs(tugSlider.value - 0.5f) * 2f;
        float resistance = Mathf.Lerp(1f, 1f - edgeResistanceFactor, distanceFromCenter);

        tugSlider.value = Mathf.Clamp01(tugSlider.value + diff * pushPower * resistance * Time.deltaTime);

        if (tugSlider.value >= 1f)
        {
            EndWithWinner(player1Index);
            return;
        }

        if (tugSlider.value <= 0f)
        {
            EndWithWinner(player2Index);
            return;
        }

        if (maxDuration > 0f && elapsed >= maxDuration)
        {
            IsActive = false;

            if (panel != null)
                panel.SetActive(false);

            OnTugOfWarTimedOut?.Invoke();
            OnEventCompleted?.Invoke(0);
        }
    }

    //<summary>
    //Returns the gyro cursor position normalized to 0-1 viewport space (screen-resolution independent), instead of raw pixels.
    //</summary>
    private Vector2 GetNormalizedPos(int deviceIndex)
    {
        return ConvertToViewportPos.GyroToNormalizedUnclamped(deviceIndex);
    }

    private float UpdateAndGetShake(int deviceIndex)
    {
        Vector2 currentPos = GetNormalizedPos(deviceIndex);
        float movement = Vector2.Distance(currentPos, prevViewportPos[deviceIndex]);
        prevViewportPos[deviceIndex] = currentPos;

        if (movement < shakeNoiseFloor)
            movement = 0f;

        float decay = Mathf.Exp(-shakeDecayRate * Time.deltaTime);
        float newIntensity = shakeIntensity[deviceIndex] * decay + movement;

        // Cap so a single big shake spike can't swing/end the match by itself -
        // sustained shaking is required to hold a strong advantage.
        shakeIntensity[deviceIndex] = Mathf.Min(newIntensity, maxShakeIntensity);

        return shakeIntensity[deviceIndex];
    }

    //<summary>
    //deviceIndex is the raw JoyCon index (player1Index/player2Index).
    //This converts it to the player's display number (1 or 2) before broadcasting/logging, so the device index and the "player number" don't get conflated.
    //</summary>
    private void EndWithWinner(int deviceIndex)
    {
        IsActive = false;

        if (panel != null)
            panel.SetActive(false);

        int playerNumber = (deviceIndex == player1Index) ? 1 : 2;

        OnTugOfWarWon?.Invoke(playerNumber);
        OnEventCompleted?.Invoke(playerNumber);
        Debug.Log("Player " + playerNumber + " wins");
    }
}