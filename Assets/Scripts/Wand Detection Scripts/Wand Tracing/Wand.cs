using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Wand : MonoBehaviour
{
    [SerializeField]
    private bool instatiated = true;
    private bool initialized = false;

    public LineRenderer lineRenderer;
    private JoyConTracker joyConTracker;

    [Space(10)]
    [Header("Controller settings if using controller")]
    public bool usingController = true;
    [Tooltip("Index 0: left, Index 1: right")]
    [Range(0, 1)]
    public int deviceIndex = 0;

    private bool UsingController;


    private List<Vector2> points = new List<Vector2>();
    private List<Vector2> catmullPoints = new List<Vector2>();
    private List<Vector2[]> renderedPoints = new List<Vector2[]>();
    private List<Vector2> renderedCatmullPoints = new List<Vector2>();
    public IReadOnlyList<Vector2> RenderedCatmullPoints => renderedCatmullPoints;

    private float zClipPlane = 0;

    private int lastProcessedIndex = -1;

    private Coroutine loggingCoroutine;
    private Coroutine deleteDrawing;

    private WandInputActions inputActions;

    public bool drawActive = false;

    //last screen-space cursor position actually used this frame, sourced from
    //whichever input path is live (controller gyro or mouse). Other systems (e.g.
    //CircleBonusEvent) should read this instead of calling ConvertToViewportPos
    //themselves, so they always match what this Wand is really drawing with.
    public Vector2 CurrentScreenPos { get; private set; }

    private void Start()
    {
        if (!instatiated)
        {
            Init();
        }
    }

    private void Update()
    {
        if (!initialized)
            return;

        while (lastProcessedIndex < points.Count - 1)
        {
            lastProcessedIndex++;
            HandleCatmullPoints(lastProcessedIndex);
        }

        if (lineRenderer != null && renderedPoints.Count > 0)
        {
            LineRendererInterface.AddPoints(lineRenderer, renderedPoints, zClipPlane);
            renderedPoints.Clear();
        }

        transform.position = GetCurrentPos();
        //Debug.Log("Wand position: " + transform.position);
    }

    private void Init()
    {
        Init(usingController, deviceIndex);
    }

    public void Init(bool usingController, int deviceIndex)
    {
        UsingController = usingController;
        this.deviceIndex = deviceIndex;

        joyConTracker = JoyConTracker.Instance;

        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            Debug.LogWarning($"{nameof(Wand)}: No LineRenderer assigned on {gameObject.name}.");
        }
        else
        {
            Color baseColor = deviceIndex == 0 ? GameConstants.LeftDrawingColor : GameConstants.RightDrawingColor;
            Color darker = baseColor * GameConstants.PlayerDrawDarkenFactor;
            darker.a = baseColor.a; // multiplying scales alpha too — restore it

            lineRenderer.startColor = darker;
            lineRenderer.endColor = darker;

            lineRenderer.startWidth = GameConstants.LineWidth;
            lineRenderer.endWidth = GameConstants.LineWidth;
        }

        if (UsingController && joyConTracker != null && joyConTracker.gameObject.activeInHierarchy && joyConTracker.count != 0)
        {
            joyConTracker.drawingButtonPressed += DrawStarted;
            joyConTracker.drawingButtonReleased += DrawCancelled;
        }
        else
        {
            if (UsingController)
            {
                Debug.LogWarning($"{nameof(Wand)}: No JoyConTracker instance found for controller input in scene.");
                UsingController = false;
            }

            inputActions = new WandInputActions();

            inputActions.Enable();

            if (deviceIndex == 0)
            {
                inputActions.Wand.DrawingLeft.started += DrawStarted;
                inputActions.Wand.DrawingLeft.canceled += DrawCancelled;
            }
            else
            {
                inputActions.Wand.DrawingRight.started += DrawStarted;
                inputActions.Wand.DrawingRight.canceled += DrawCancelled;
            }
        }

        initialized = true;

        //register with CircleBonusEvent (if the mid-game event exists yet)
        //so it can read this wand's drawActive/CurrentScreenPos directly. Wands
        //are spawned at runtime in the gameplay scene, so there's no Inspector
        //slot to drag this into — registration has to happen here instead.
        CircleBonusEvent.Instance?.RegisterWand(this);
    }

    public Vector2[] listToArray(List<Vector3> list)
    {
        Vector2[] result = new Vector2[list.Count];

        int i = 0;
        foreach (Vector3 item in list)
        {
            result[i] = new Vector2(item.x, item.y);
            i++;
        }

        return result;
    }

    #region Subscribe/Unsubscribe To Input System
    private void OnDisable()
    {
        if (UsingController)
        {
            if (joyConTracker != null && joyConTracker.gameObject.activeInHierarchy)
            {
                joyConTracker.drawingButtonPressed -= DrawStarted;
                joyConTracker.drawingButtonReleased -= DrawCancelled;
            }
        }
        else
        {
            if (deviceIndex == 0)
            {
                inputActions.Wand.DrawingLeft.started -= DrawStarted;
                inputActions.Wand.DrawingLeft.canceled -= DrawCancelled;
            }
            else
            {
                inputActions.Wand.DrawingRight.started -= DrawStarted;
                inputActions.Wand.DrawingRight.canceled -= DrawCancelled;
            }

            inputActions.Disable();
        }

        //mirror the registration in Init() so CircleBonusEvent never holds
        //a stale reference to a wand that's been destroyed/despawned.
        CircleBonusEvent.Instance?.UnregisterWand(this);

        StopAllCoroutines();
    }
    #endregion


    #region Interpolate the points (Catmull-Rom curve)
    private void HandleCatmullPoints(int n)
    {
        if (n == 1)
        {
            catmullPoints.Add(points[0]);
            catmullPoints.Add(points[0]);
        }

        if (n < 2) return;

        catmullPoints.Add(points[n - 1]);
        if (PointsManipulation.Angle(points[n - 2], points[n - 1], points[n]) > GameConstants.sharpAngleThreshold)
            catmullPoints.Add(points[n - 1]);

        if (catmullPoints.Count < 4) return;

        int m = catmullPoints.Count - 1;

        renderedPoints.Add(
            PointsManipulation.EvaluateCatmullSegment(
                catmullPoints[m - 3],
                catmullPoints[m - 2],
                catmullPoints[m - 1],
                catmullPoints[m],
                GameConstants.CatmullResolution
                ));
    }

    private void FlushCatmullTail()
    {
        if (catmullPoints.Count < 4) return;

        Vector2 last = points[^1];
        if (catmullPoints[^1] != last)
            catmullPoints.Add(last);

        int m = catmullPoints.Count - 1;

        Vector2[] segment = PointsManipulation.EvaluateCatmullSegment(
            catmullPoints[m - 3],
            catmullPoints[m - 2],
            catmullPoints[m - 1],
            catmullPoints[m],
            GameConstants.CatmullResolution
        );

        renderedPoints.Add(segment);
        //LineRendererInterface.RemovePoint(lineRenderer);  
        LineRendererInterface.AddPoints(lineRenderer, renderedPoints, zClipPlane);

        renderedPoints.Clear();
    }
    #endregion


    #region Functions to find and record position of cursor
    private Vector3 GetCurrentPos()
    {
        Vector2 screenPos;

        Camera mainCam = Camera.main;

        if (UsingController)
        {
            screenPos = ConvertToViewportPos.GyroToViewPort(deviceIndex);
            //Debug.Log("Controller viewport pos: " + screenPos);
        }
        else
        {
            screenPos = ConvertToViewportPos.MousePosToViewPort(inputActions.Wand.Position.ReadValue<Vector2>(), deviceIndex == 0);
            //Debug.Log("Mouse screen pos: " + screenPos);
        }

        //record whichever position was actually used this frame so other
        //systems (e.g. CircleBonusEvent) can read the real cursor pos instead of
        //re-deriving it (and getting it wrong when we're in mouse mode).
        CurrentScreenPos = screenPos;

        return mainCam.ScreenToWorldPoint(new Vector3(
            screenPos.x,
            screenPos.y,
            mainCam.nearClipPlane + GameConstants.DistanceToCamera
            ));
    }

    /// <summary>
    /// Record location of cursor in "points" Queue data struct.
    /// </summary>
    private void TryRecordCurrentPos(Vector2 newPos)
    {
        if (points.Count == 0 || newPos != points[^1])
            points.Add(newPos);
    }

    /// <summary>
    /// Every sampleSpeedSec duration,
    /// the location of the cursor is recorded.
    /// </summary>
    private IEnumerator LogMousePos()
    {
        zClipPlane = Camera.main.gameObject.transform.position.z + GameConstants.DistanceToCamera;
        while (drawActive)
        {
            TryRecordCurrentPos(GetCurrentPos());

            yield return new WaitForSeconds(GameConstants.sampleSpeedSec);
        }
    }
    #endregion


    #region Events to send drawing data
    public event Action<Vector2[]> OnDrawingComplete;

    //fired the instant a draw starts/stops, regardless of input source
    //(controller or mouse). CircleBonusEvent subscribes to these instead of
    //JoyConTracker's events so bonus-eligibility tracking works for both.
    public event Action OnDrawStarted;
    public event Action OnDrawStopped;

    public void SendData()
    {
        if (!drawActive && points.Count > 1)
        {
            //Debug.Log("Drawing finished");
            OnDrawingComplete?.Invoke(points.ToArray());
        }
    }
    #endregion


    #region Input System Helper Functions
    //handle -1 for when not using controller
    private void DrawStarted(int handle = -1)
    {
        if (handle != -1 && handle != deviceIndex)
            return;

        drawActive = true;
        OnDrawStarted?.Invoke(); // NEW

        points.Clear();
        catmullPoints.Clear();
        renderedPoints.Clear();
        lastProcessedIndex = -1;

        if (deleteDrawing != null)
            StopCoroutine(deleteDrawing);

        LineRendererInterface.ResetLineRenderer(lineRenderer);

        if (loggingCoroutine != null)
            StopCoroutine(loggingCoroutine);
        loggingCoroutine = StartCoroutine(LogMousePos());
    }
    private void DrawStarted(InputAction.CallbackContext callback)
    {
        DrawStarted();
    }

    private void DrawCancelled(int handle = -1)
    {
        if (handle != -1 && handle != deviceIndex)
            return;

        if (loggingCoroutine != null)
            StopCoroutine(loggingCoroutine);

        deleteDrawing = StartCoroutine(EraseBackground());

        drawActive = false;
        //fire before Flush/SendData so anything listening for "did the
        //in-progress draw hit its bonus target" (e.g. CircleBonusEvent) can
        //check drawActive/CurrentScreenPos state consistently before the
        //drawing is finalized.
        OnDrawStopped?.Invoke();

        FlushCatmullTail();
        SendData();
    }

    private void DrawCancelled(InputAction.CallbackContext callback)
    {
        DrawCancelled();
    }

    private IEnumerator EraseBackground()
    {
        yield return new WaitForSeconds(3f);

        lineRenderer.positionCount = 0;
    }
    #endregion

    public void ClearDrawnLine()
    {
        if (lineRenderer != null)
            lineRenderer.positionCount = 0;

        if (deleteDrawing != null)
        {
            StopCoroutine(deleteDrawing);
            deleteDrawing = null;
        }
    }
}