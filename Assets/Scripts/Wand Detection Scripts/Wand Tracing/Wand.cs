using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Wand : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public GameObject wandObject;

    public bool usingController = false;

    [Tooltip("Individual joycon connection id")]
    [Range(0, 1)]
    public int deviceId = 0;
    private bool UsingController;

    private bool controllerActive = false;

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
        UsingController = usingController;

        if (!UsingController)
        {
            inputActions = new WandInputActions();
            Cursor.visible = GameConstants.CursorVisible;
        }

        mainCam = Camera.main;

    }

    private void Start()
    {
        if (lineRenderer == null)
        {
            Debug.LogError($"{nameof(Wand)}: No LineRenderer assigned on {gameObject.name}.");
            Debug.Log("Setting game object to inactive.");
            gameObject.SetActive(false);
            return;
        }

        lineRenderer.startWidth = GameConstants.LineWidth;
        lineRenderer.endWidth = GameConstants.LineWidth;

        if (UsingController && JoyConTracker.Instance == null)
        {
            Debug.LogError($"{nameof(Wand)}: No JoyConTracker instance found for controller input on {gameObject.name}.");
            Debug.Log("Setting game object to inactive.");
            gameObject.SetActive(false);
            return;
        }

        if (UsingController)
        {
            JoyConTracker.Instance.drawingButtonPressed += DrawStarted;
            JoyConTracker.Instance.drawingButtonReleased += DrawCancelled;
        }
        else
        {
            inputActions.Enable();
            inputActions.Wand.DrawingLeft.started += DrawStarted;
            inputActions.Wand.DrawingLeft.canceled += DrawCancelled;
        }
    }

    private void Update()
    {
        if (usingController && !controllerActive)
        {
            controllerActive = JoyConTracker.Instance.Connected;
            return;
        }

        while (lastProcessedIndex < points.Count - 1)
        {
            lastProcessedIndex++;
            HandleCatmullPoints(lastProcessedIndex);
        }

        if (renderedPoints.Count > 0)
        {
            LineRendererInterface.AddPoints(lineRenderer, renderedPoints);
            renderedPoints.Clear();
        }

        if (wandObject != null)
        {
            wandObject.transform.position = GetCurrentPos();
            //wandObject.transform.rotation = Quaternion.Euler(GameConstants.QuatRotation) * mainCam.transform.rotation;
        }
    }


    #region Subscribe/Unsubscribe To Input System
    private void OnDisable()
    {
        if (UsingController)
        {
            JoyConTracker.Instance.drawingButtonPressed -= DrawStarted;
            JoyConTracker.Instance.drawingButtonReleased -= DrawCancelled;
        }
        else
        {
            inputActions.Wand.DrawingLeft.started -= DrawStarted;
            inputActions.Wand.DrawingLeft.canceled -= DrawCancelled;
            inputActions.Disable();
        }

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
        LineRendererInterface.AddPoints(lineRenderer, renderedPoints);

        renderedPoints.Clear();
    }
    #endregion


    #region Functions to find and record position of cursor
    private Vector3 GetCurrentPos()
    {
        Vector2 screenPos;

        if (UsingController)
        {
            screenPos = ConvertToViewportPos.Caculate(0);
            //Debug.Log("Controller viewport pos: " + screenPos);
        }
        else
        {
            screenPos = inputActions.Wand.PositionLeft.ReadValue<Vector2>();
            //Debug.Log("Mouse screen pos: " + screenPos);
        }

        return mainCam.ScreenToWorldPoint(new Vector3(
            screenPos.x,
            screenPos.y,
            Camera.main.nearClipPlane + GameConstants.DistanceToCamera
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
        while (drawActive)
        {
            TryRecordCurrentPos(GetCurrentPos());

            yield return new WaitForSeconds(GameConstants.sampleSpeedSec);
        }
    }
    #endregion
    

    #region Events to send drawing data
    public event Action<Vector2[]> OnDrawingComplete;

    public void SendData()
    {
        if (!drawActive && points.Count > 1)
            OnDrawingComplete?.Invoke(points.ToArray());
    }
    #endregion


    #region Input System Helper Functions
    private void DrawStarted()
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
    private void DrawStarted(InputAction.CallbackContext callback)
    {
        DrawStarted();
    }

    private void DrawCancelled()
    {
        if (loggingCoroutine != null)
            StopCoroutine(loggingCoroutine);    

        drawActive = false;

        FlushCatmullTail();
        SendData();
    }

    private void DrawCancelled(InputAction.CallbackContext callback)
    {
        DrawCancelled();
    }
    #endregion
}