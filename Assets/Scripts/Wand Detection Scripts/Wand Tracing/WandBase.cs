using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class WandBase : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float sampleSpeedSec = 0.01f;
    public float lineWidth = 0.1f;

    public WandCursorInitialise wandCursor;


    protected Queue<Vector2> points = new Queue<Vector2>();

    protected Coroutine loggingCoroutine;

    private WandInputActions inputActions;
    protected bool drawActive = false;

    private Camera mainCam;

    #region Public Interface
    private bool DataReady => !drawActive && points.Count > 1;
    public Vector2[] Data => points.ToArray();
    #endregion


    private void Awake()
    {
        inputActions = new WandInputActions();
        mainCam = Camera.main;
    }

    protected virtual void Start()
    {
        if (lineRenderer == null)
        {
            Debug.LogError($"{nameof(WandBase)}: No LineRenderer assigned on {gameObject.name}.");
            return;
        }

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        if (wandCursor == null)
        {
            wandCursor = GetComponent<WandCursorInitialise>();
        }
    }

    private void Update()
    {
        if (CheckPointsHaveChanged())
        {
            UpdateLineRenderer();
        }
    }

    #region Subscribe/Unsubscribe To Input System
    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Wand.Drawing.started += DrawStarted;
        inputActions.Wand.Drawing.canceled += DrawCanceled;
    }
    protected virtual void OnDisable()
    {
        inputActions.Wand.Drawing.started -= DrawStarted;
        inputActions.Wand.Drawing.canceled -= DrawCanceled;
        inputActions.Disable();
    }
    #endregion


    #region LineRenderer interface
    private bool CheckPointsHaveChanged()
    {
        return points.Count != lineRenderer.positionCount;
    }

    private void UpdateLineRenderer()
    {
        // A point could be added while this function is running
        Vector2[] snapshot = points.ToArray();
        lineRenderer.positionCount = snapshot.Length;

        for (int i = 0; i < snapshot.Length; i++)
        {
            lineRenderer.SetPosition(i, snapshot[i]);
        }
    }
    #endregion


    #region Functions to find and record position of cursor
    /// <summary>
    /// Record location of cursor in "points" Queue data struct.
    /// </summary>
    private void RecordCurrentPos()
    {
        if (wandCursor != null)
        {
            points.Enqueue(wandCursor.pos2);
            return;
        }
        else
        {
            Vector2 mousePosition = inputActions.Wand.Position.ReadValue<Vector2>();
            // Debug.Log($"Mouse Posiiton: {mousePosition}"); 
            
            points.Enqueue(mainCam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y)));
        }

        OnPointRecorded();
    }

    /// <summary>
    /// Invoked when a point is recorded.
    /// </summary>
    protected virtual void OnPointRecorded()
    {
    }

    /// <summary>
    /// Every sampleSpeedSec duration,
    /// the location of the cursor is recorded.
    /// </summary>
    protected IEnumerator LogMousePos()
    {
        while (drawActive)
        {
            RecordCurrentPos();

            yield return new WaitForSeconds(sampleSpeedSec);
        }
    }
    #endregion


    #region Events to send drawing data
    public static event Action<Vector2[]> OnDrawingComplete;

    public void SendData()
    {
        if (DataReady)
            OnDrawingComplete?.Invoke(points.ToArray());
    }
    #endregion


    #region Input System Helper Functions
    protected virtual void DrawStarted(InputAction.CallbackContext callback)
    {
        drawActive = true;
    }
    protected virtual void DrawCanceled(InputAction.CallbackContext callback)
    {
        drawActive = false;

        SendData();
    }
    #endregion
}