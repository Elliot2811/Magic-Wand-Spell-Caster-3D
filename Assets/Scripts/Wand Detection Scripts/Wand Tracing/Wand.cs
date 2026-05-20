using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WandBase : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public WandCursorInitialise wandCursor;


    private List<Vector2> points = new List<Vector2>();
    private List<Vector2> catmullPoints = new List<Vector2>();
    private List<Vector2[]> renderedPoints = new List<Vector2[]>();

    private int lastProcessedIndex = -1;


    private Coroutine loggingCoroutine;

    private WandInputActions inputActions;
    private bool drawActive = false;

    private Camera mainCam;


    private void Awake()
    {
        inputActions = new WandInputActions();
        mainCam = Camera.main;
    }

    private void Start()
    {
        if (lineRenderer == null)
        {
            Debug.LogError($"{nameof(WandBase)}: No LineRenderer assigned on {gameObject.name}.");
            return;
        }

        lineRenderer.startWidth = GameConstants.LineWidth;
        lineRenderer.endWidth = GameConstants.LineWidth;

        if (wandCursor == null)
        {
            wandCursor = GetComponent<WandCursorInitialise>();
        }
    }

    private void Update()
    {
        while (lastProcessedIndex < points.Count - 1)
        {
            lastProcessedIndex++;
            HandleCatmullPoints(lastProcessedIndex);
        }

        if (renderedPoints.Count > 0)
        {
            //LineRendererInterface.RemovePoint(lineRenderer);
            LineRendererInterface.AddPoints(lineRenderer, renderedPoints);
            //LineRendererInterface.AddPoint(lineRenderer, points[^1]);
            renderedPoints.Clear();
        }
    }


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
        LineRendererInterface.AddPoints(lineRenderer, renderedPoints);

        renderedPoints.Clear();
    }
    #endregion


    #region Subscribe/Unsubscribe To Input System
    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Wand.Drawing.started += DrawStarted;
        inputActions.Wand.Drawing.canceled += DrawCanceled;
    }
    private void OnDisable()
    {
        inputActions.Wand.Drawing.started -= DrawStarted;
        inputActions.Wand.Drawing.canceled -= DrawCanceled;
        inputActions.Disable();
    }
    #endregion


    #region Functions to find and record position of cursor
    /// <summary>
    /// Record location of cursor in "points" Queue data struct.
    /// </summary>
    private void RecordCurrentPos()
    {
        Vector2 newPos;

        if (wandCursor != null)
        {
            newPos = wandCursor.pos2;
        }
        else
        {
            Vector2 mousePosition = inputActions.Wand.Position.ReadValue<Vector2>();
            // Debug.Log($"Mouse Posiiton: {mousePosition}"); 

            newPos = mainCam.ScreenToWorldPoint(mousePosition);
        }

        if (points.Count == 0 || newPos != points[^1])
            points.Add(newPos);
    }

    /// <summary>
    /// Every sampleSpeedSec duration,
    /// the location of the cursor is recorded.
    /// </summary>
    private IEnumerator LogMousePos()
    {
        while (drawActive)
        {
            RecordCurrentPos();

            yield return new WaitForSeconds(GameConstants.sampleSpeedSec);
        }
    }
    #endregion
    

    #region Events to send drawing data
    public static event Action<Vector2[]> OnDrawingComplete;

    public void SendData()
    {
        if (!drawActive && points.Count > 1)
            OnDrawingComplete?.Invoke(points.ToArray());
    }
    #endregion


    #region Input System Helper Functions
    private void DrawStarted(InputAction.CallbackContext callback)
    {
        drawActive = true;

        points.Clear();
        catmullPoints.Clear();
        renderedPoints.Clear();
        lastProcessedIndex = -1;

        LineRendererInterface.ResetLineRenderer(lineRenderer);

        if (loggingCoroutine != null)
            StopCoroutine(loggingCoroutine);
        loggingCoroutine = StartCoroutine(LogMousePos());
    }
    private void DrawCanceled(InputAction.CallbackContext callback)
    {
        if (loggingCoroutine != null)
            StopCoroutine(loggingCoroutine);    

        drawActive = false;

        FlushCatmullTail();
        SendData();
    }
    #endregion
}