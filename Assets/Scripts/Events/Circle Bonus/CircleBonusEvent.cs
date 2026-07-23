using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//<summary>
//Mid-match "hit the circle" minigame. A target circle floats around in each
//player's drawing area; if a player's draw passes through their circle, that
//cast becomes bonus-eligible (SpellBook should call ConsumeBonus() when it
//resolves damage for a cast). No winner — always resolves as a draw (0),
//and unlike TugOfWar, it does NOT pause the main match timer.

//Assumes circles are UI Images under a Screen Space - Overlay canvas, so
//world position == screen pixels (matches ConvertToViewportPos.GyroToViewPort).
//</summary>
public class CircleBonusEvent : MonoBehaviour, IMidGameEvent
{
    public static CircleBonusEvent Instance { get; private set; }

    [Header("Bonus")]
    public float bonusDamageMultiplier = 2f;

    [Header("UI References")]
    public GameObject panel;
    public RectTransform leftCircle;
    public RectTransform rightCircle;
    public Image leftCircleImage;
    public Image rightCircleImage;

    [Header("Tuning")]
    public float eventDuration = 25f;
    public float circleMoveSpeed = 10f;      //pixels/sec while wandering
    public float retargetIntervalMin = 3f;
    public float retargetIntervalMax = 6f;
    public Color hitFlashColor = Color.white;
    public float hitFlashDuration = 0.15f;

    [Header("Visual")]
    [Range(0f, 1f)] public float circleIdleAlpha = 0.35f; //let the spell drawing show through the circle

    [Header("Testing")]
    public bool autoStartForTesting = false;

    public bool IsActive { get; private set; }
    public bool PausesMainTimer => false;

    public event Action<int> OnEventCompleted;   //always invoked with 0
    public event Action<int> OnCircleHit;        //playerNumber, for FX/sound hooks

    private JoyConTracker tracker;
    private float elapsed;

    //index 0 = player1 (left), index 1 = player2 (right)
    private bool[] touchedCircleThisDraw = new bool[2];
    private bool[] pendingBonus = new bool[2];
    private bool[] circleGlowing = new bool[2];

    private Wand[] playerWands = new Wand[2];

    public void RegisterWand(Wand wand)
    {
        if (wand == null) return;

        int playerIndex = wand.deviceIndex;
        if (playerIndex != 0 && playerIndex != 1)
        {
            Debug.LogWarning($"CircleBonusEvent: RegisterWand got deviceIndex {playerIndex}, expected 0 or 1.");
            return;
        }

        UnsubscribeWand(playerIndex); //in case a wand is re-registered (e.g. respawned) without unregistering first
        playerWands[playerIndex] = wand;
        SubscribeWand(playerIndex);
    }

    public void UnregisterWand(Wand wand)
    {
        if (wand == null) return;

        int playerIndex = wand.deviceIndex;
        if (playerIndex != 0 && playerIndex != 1) return;

        if (playerWands[playerIndex] == wand)
        {
            UnsubscribeWand(playerIndex);
            playerWands[playerIndex] = null;
        }
    }

    private void SubscribeWand(int playerIndex)
    {
        Wand wand = playerWands[playerIndex];
        if (wand == null) return;

        if (playerIndex == 0)
        {
            wand.OnDrawStarted += HandleP1DrawStarted;
            wand.OnDrawStopped += HandleP1DrawStopped;
        }
        else
        {
            wand.OnDrawStarted += HandleP2DrawStarted;
            wand.OnDrawStopped += HandleP2DrawStopped;
        }
    }

    private void UnsubscribeWand(int playerIndex)
    {
        Wand wand = playerWands[playerIndex];
        if (wand == null) return;

        if (playerIndex == 0)
        {
            wand.OnDrawStarted -= HandleP1DrawStarted;
            wand.OnDrawStopped -= HandleP1DrawStopped;
        }
        else
        {
            wand.OnDrawStarted -= HandleP2DrawStarted;
            wand.OnDrawStopped -= HandleP2DrawStopped;
        }
    }

    //velocity (pixels/sec) each circle is currently drifting at, used for bounce-off-edge movement
    private Vector2 leftVelocity, rightVelocity;
    private float leftRetargetTimer, rightRetargetTimer;
    private Color leftBaseColor, rightBaseColor;

    private void Awake()
    {
        Instance = this;

        tracker = JoyConTracker.Instance;
        if (panel != null) panel.SetActive(false);

        //make circles translucent (so drawing underneath stays visible) and non-blocking for input
        if (leftCircleImage != null)
        {
            leftCircleImage.raycastTarget = false;
            Color c = leftCircleImage.color; c.a = circleIdleAlpha; leftCircleImage.color = c;
            leftBaseColor = leftCircleImage.color;
        }
        if (rightCircleImage != null)
        {
            rightCircleImage.raycastTarget = false;
            Color c = rightCircleImage.color; c.a = circleIdleAlpha; rightCircleImage.color = c;
            rightBaseColor = rightCircleImage.color;
        }
    }

    private void Start()
    {
        if (autoStartForTesting)
            StartCoroutine(AutoStartWhenReady());
    }

    private IEnumerator AutoStartWhenReady()
    {
        while (tracker == null || !tracker.readyToConnect)
        {
            tracker = JoyConTracker.Instance;
            yield return null;
        }
        StartEvent();
    }

    private void OnEnable()
    {
        if (tracker == null) tracker = JoyConTracker.Instance;
    }

    private void OnDisable()
    {
        //unsubscribe from whichever wands happen to be registered right now,
        //so we don't leak handlers if this component is disabled mid-match.
        UnsubscribeWand(0);
        UnsubscribeWand(1);
    }

    public void StartEvent()
    {
        if (tracker == null) tracker = JoyConTracker.Instance;

        if (leftCircle == null || rightCircle == null)
        {
            Debug.LogError("CircleDrawEvent: leftCircle/rightCircle not assigned in the Inspector.");
            return;
        }

        //(re)discover both players' Wands right as the event starts, instead
        //of relying purely on each Wand finding CircleBonusEvent.Instance during
        //its own Init(). Wands are spawned at runtime in another scene, so that
        //ordering isn't guaranteed — but by the time this mid-match event fires,
        //both players' Wands are guaranteed to already exist, so scanning for
        //them here is a reliable fallback (and covers a Wand that registered
        //before this component's Instance existed, or never registered at all).
        DiscoverWands();

        elapsed = 0f;
        Array.Clear(touchedCircleThisDraw, 0, touchedCircleThisDraw.Length);
        pendingBonus[0] = false;
        pendingBonus[1] = false;

        PositionCircle(leftCircle, GetRandomPointInBounds(true));
        PositionCircle(rightCircle, GetRandomPointInBounds(false));
        PickNewTarget(true);
        PickNewTarget(false);

        if (panel != null) panel.SetActive(true);
        IsActive = true;

        Debug.Log($"CircleDrawEvent: started, duration={eventDuration}s. Main timer keeps running.");
    }

    //scans the currently loaded scene(s) for Wand components and registers
    //whichever ones it finds. Safe to call more than once — RegisterWand already
    //no-ops/re-subscribes cleanly if a wand is already registered.
    private void DiscoverWands()
    {
        Wand[] foundWands = FindObjectsOfType<Wand>();
        foreach (Wand wand in foundWands)
            RegisterWand(wand);

        if (playerWands[0] == null)
            Debug.LogWarning("CircleBonusEvent: no Wand found with deviceIndex 0 (player1/left) when the event started.");
        if (playerWands[1] == null)
            Debug.LogWarning("CircleBonusEvent: no Wand found with deviceIndex 1 (player2/right) when the event started.");
    }

    private void Update()
    {
        if (!IsActive) return;

        elapsed += Time.deltaTime;

        WanderCircle(true, leftCircle);
        WanderCircle(false, rightCircle);

        CheckHit(playerWands[0], leftCircle, 1);
        CheckHit(playerWands[1], rightCircle, 2);

        if (elapsed >= eventDuration)
            EndEvent();
    }

    private void EndEvent()
    {
        IsActive = false;
        if (panel != null) panel.SetActive(false);
        pendingBonus[0] = false;//clear any bonus armed but never consumed before the event ended
        pendingBonus[1] = false;
        Debug.Log("CircleDrawEvent: time's up, ending with no winner.");
        OnEventCompleted?.Invoke(0);
    }

    #region Draw tracking

    //thin wrappers so each Wand's parameterless events can carry which
    //player index they belong to.
    private void HandleP1DrawStarted() => HandleDrawStarted(0);
    private void HandleP1DrawStopped() => HandleDrawStopped(0);
    private void HandleP2DrawStarted() => HandleDrawStarted(1);
    private void HandleP2DrawStopped() => HandleDrawStopped(1);

    private void HandleDrawStarted(int playerIndex)
    {
        if (!IsActive) return;
        touchedCircleThisDraw[playerIndex] = false;
        pendingBonus[playerIndex] = false; //reset so a bonus armed on a previous draw doesn't carry into this one
    }

    private void HandleDrawStopped(int playerIndex)
    {
        if (!IsActive) return;

        //Bonus is only armed here, once the draw has actually finished, and only
        //if CheckHit confirmed a real pass-through while that draw was live —
        //this is what stops a bonus being granted just for the circle happening
        //to be near the cursor at some unrelated moment.
        if (touchedCircleThisDraw[playerIndex])
            pendingBonus[playerIndex] = true;
    }

    private void CheckHit(Wand wand, RectTransform circle, int playerNumber)
    {
        int playerIndex = playerNumber - 1;

        if (!IsActive || wand == null || !wand.drawActive)
        {
            //drawing stopped entirely — revert to idle and free the circle to relocate
            EndGlow(playerIndex, playerNumber);
            return;
        }

        Vector2 cursorScreenPos = wand.CurrentScreenPos;
        Vector2 circleScreenPos = circle.position;
        bool isInside = Vector2.Distance(cursorScreenPos, circleScreenPos) <= GetCircleRadius(circle);

        //live containment drives the bonus directly each frame, so moving back
        //out of the circle before releasing cancels the bonus instead of it
        //staying armed forever once touched
        pendingBonus[playerIndex] = isInside;

        if (isInside)
        {
            if (!circleGlowing[playerIndex])
            {
                //cursor just entered — light up and hold, instead of a timed flash
                circleGlowing[playerIndex] = true;
                SetCircleColor(playerNumber, hitFlashColor);

                if (!touchedCircleThisDraw[playerIndex])
                {
                    touchedCircleThisDraw[playerIndex] = true;
                    OnCircleHit?.Invoke(playerNumber);
                }
            }
        }
        else if (circleGlowing[playerIndex])
        {
            //cursor just left the circle while still drawing — revert and relocate now
            EndGlow(playerIndex, playerNumber);
        }
    }

    //<summary>
    //Reverts a circle to its idle color and relocates it — called once the player
    //leaves the circle or stops drawing entirely, rather than on a fixed timer.
    //</summary>
    private void EndGlow(int playerIndex, int playerNumber)
    {
        if (!circleGlowing[playerIndex])
            return;

        circleGlowing[playerIndex] = false;
        SetCircleColor(playerNumber, playerNumber == 1 ? leftBaseColor : rightBaseColor);
        PickNewTarget(playerNumber == 1); //relocate now that the interaction's actually over
    }

    private void SetCircleColor(int playerNumber, Color color)
    {
        Image img = playerNumber == 1 ? leftCircleImage : rightCircleImage;
        if (img != null)
            img.color = color;
    }

    //private IEnumerator FlashCircle(Image img, Color baseColor, RectTransform circleTransform)
    //{
    //    if (img == null) yield break;

    //    Vector3 baseScale = circleTransform != null ? circleTransform.localScale : Vector3.one;
    //    Vector3 punchScale = baseScale * 1.4f;

    //    img.color = hitFlashColor;
    //    if (circleTransform != null) circleTransform.localScale = punchScale;

    //    yield return new WaitForSeconds(hitFlashDuration);

    //    img.color = baseColor;
    //    if (circleTransform != null) circleTransform.localScale = baseScale;
    //}

    #endregion

    #region Bonus API — call from SpellBook when resolving a cast's damage

    //<summary>
    //playerNumber: 1 = left, 2 = right. Returns true (and clears the flag) if
    //that player's most recent completed draw passed through their circle.
    //</summary>
    public bool ConsumeBonus(int playerNumber)
    {
        if (!IsActive) return false; //no bonus once the event has ended
        if (playerNumber != 1 && playerNumber != 2) return false;
        bool had = pendingBonus[playerNumber - 1];
        pendingBonus[playerNumber - 1] = false;
        return had;
    }

    #endregion

    #region Circle movement

    //picks a brand new random drift direction at full circleMoveSpeed;
    //used on start and right after a hit so the circle doesn't sit still and get re-tapped
    private void PickNewTarget(bool left)
    {
        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 vel = dir * circleMoveSpeed;

        if (left) { leftVelocity = vel; leftRetargetTimer = UnityEngine.Random.Range(retargetIntervalMin, retargetIntervalMax); }
        else { rightVelocity = vel; rightRetargetTimer = UnityEngine.Random.Range(retargetIntervalMin, retargetIntervalMax); }
    }

    //small periodic turn (not a full redirect) so the drift curves gently instead of
    //travelling in one dead-straight line between bounces
    private void NudgeDirection(bool left)
    {
        Vector2 vel = left ? leftVelocity : rightVelocity;
        float currentAngle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
        float turn = UnityEngine.Random.Range(-45f, 45f);
        float newAngle = (currentAngle + turn) * Mathf.Deg2Rad;
        Vector2 newVel = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle)) * circleMoveSpeed;

        if (left) { leftVelocity = newVel; leftRetargetTimer = UnityEngine.Random.Range(retargetIntervalMin, retargetIntervalMax); }
        else { rightVelocity = newVel; rightRetargetTimer = UnityEngine.Random.Range(retargetIntervalMin, retargetIntervalMax); }
    }

    private void WanderCircle(bool left, RectTransform circle)
    {
        if (left) { leftRetargetTimer -= Time.deltaTime; if (leftRetargetTimer <= 0f) NudgeDirection(true); }
        else { rightRetargetTimer -= Time.deltaTime; if (rightRetargetTimer <= 0f) NudgeDirection(false); }

        Vector2 velocity = left ? leftVelocity : rightVelocity;
        Vector2 pos = circle.position;
        pos += velocity * Time.deltaTime;

        Vector2 min, max;
        GetScreenBounds(left, out min, out max);

        //inset by the circle's own radius so the visible edge bounces at the wall, not the center
        float r = GetCircleRadius(circle);
        min += new Vector2(r, r);
        max -= new Vector2(r, r);

        //bounce: clamp to the wall and flip the velocity component that hit it
        if (pos.x < min.x) { pos.x = min.x; velocity.x = Mathf.Abs(velocity.x); }
        else if (pos.x > max.x) { pos.x = max.x; velocity.x = -Mathf.Abs(velocity.x); }

        if (pos.y < min.y) { pos.y = min.y; velocity.y = Mathf.Abs(velocity.y); }
        else if (pos.y > max.y) { pos.y = max.y; velocity.y = -Mathf.Abs(velocity.y); }

        if (left) leftVelocity = velocity; else rightVelocity = velocity;

        PositionCircle(circle, pos);
    }

    //converts the normalized GameConstants drawing rect into screen-pixel min/max for bounce checks
    private void GetScreenBounds(bool left, out Vector2 min, out Vector2 max)
    {
        Rect bounds = left ? GameConstants.DrawingRectLeft : GameConstants.DrawingRectRight;
        min = new Vector2(bounds.xMin * Screen.width, bounds.yMin * Screen.height);
        max = new Vector2(bounds.xMax * Screen.width, bounds.yMax * Screen.height);
    }

    //random starting position within a player's drawing area, used once at StartEvent
    private Vector2 GetRandomPointInBounds(bool left)
    {
        Rect bounds = left ? GameConstants.DrawingRectLeft : GameConstants.DrawingRectRight;
        float x = UnityEngine.Random.Range(bounds.xMin, bounds.xMax) * Screen.width;
        float y = UnityEngine.Random.Range(bounds.yMin, bounds.yMax) * Screen.height;
        return new Vector2(x, y);
    }

    //derives the circle's actual on-screen radius from its RectTransform instead of a hand-tuned value
    private float GetCircleRadius(RectTransform circle) => (circle.rect.width * 0.5f) * circle.lossyScale.x;

    private void PositionCircle(RectTransform circle, Vector2 screenPos) => circle.position = screenPos;
    #endregion
}