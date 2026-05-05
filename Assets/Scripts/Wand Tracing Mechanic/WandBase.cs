using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class WandBase : MonoBehaviour
{
    #region Global Variables

    #region Inspector Variables
    public LineRenderer lineRenderer;
    public float sampleSpeedSec = 0.01f;
    public float lineWidth = 0.1f;

    public WandCursorInitialise wandCursor;
    #endregion

    #region Runtime Variables
    protected WandInputActions inputActions;
    protected Camera mainCam;
    protected bool ortographicView = true;

    protected bool drawHeld;
    protected bool drawStart;

    protected Queue<Vector2> points = new Queue<Vector2>();
    #endregion

    #endregion

    #region Getter Setters
    public Queue<Vector2> GetPoints
    {
        get {  return points; }
    }
    #endregion

    #region MonoBehaviour Functions
    private void Awake()
    {
        inputActions = new WandInputActions();
        mainCam = Camera.main;
    }

    protected virtual void Start()
    {
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        if (wandCursor != null)
        {
            wandCursor = GetComponent<WandCursorInitialise>();
        }

        ortographicView = Camera.main.orthographic;
    }

    protected virtual void Update()
    {
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Wand.Drawing.started += DrawStarted;
        inputActions.Wand.Drawing.performed += DrawPerformed;
        inputActions.Wand.Drawing.canceled += DrawCanceled;
    }

    private void OnDisable()
    {
        inputActions.Wand.Drawing.started -= DrawStarted;
        inputActions.Wand.Drawing.performed -= DrawPerformed;
        inputActions.Wand.Drawing.canceled -= DrawCanceled;
        inputActions.Disable();
    }
    #endregion

    #region Normal Functions
    protected virtual bool CheckLineRenedererNeedsUpdate()
    {
        return points.Count != lineRenderer.positionCount;
    }

    protected virtual void UpdateLineRenderer()
    {
        lineRenderer.positionCount = points.Count;

        Queue<Vector2>.Enumerator enumerator = points.GetEnumerator();
        enumerator.MoveNext(); // Move to the first element
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, enumerator.Current);
            enumerator.MoveNext();
        }
    }

    protected void FindInsertWorldPos()
    {
        if (wandCursor != null)
        {
            // Do something 
            points.Enqueue(wandCursor.pos2);
        }

        Vector2 mousePosition = inputActions.Wand.Position.ReadValue<Vector2>();
        //Debug.Log("Mouse Position: " + mousePosition);

        points.Enqueue(ortographicView ? WorldPosOrthographic(mousePosition) : WorldPosPerspective(mousePosition));
    }

    protected Vector3 WorldPosOrthographic(Vector2 mousePos)
    {
        Vector2 worldPos = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y));
        return worldPos;
    }

    protected Vector3 WorldPosPerspective(Vector2 mousePos)
    {
        return new Vector3(0f, 0f, 0f);
    }

    public virtual bool dataReady()
    {
        return !drawHeld && points.Count > 1;
    }

    public virtual Vector2[] RequestData()
    {
        return points.ToArray();
    }

    public virtual Vector2[] RequestData(bool dataResampled, int numPoints)
    {
        if (!dataResampled)
            return points.ToArray();

        InputResampler inputResampler = new InputResampler(points, numPoints);
        inputResampler.ResampleData();
        inputResampler.NormalizePoints();
        return inputResampler.getPoints;
    }

    protected virtual IEnumerator LogMousePos()
    {
        while (drawHeld)
        {
            FindInsertWorldPos();
            UpdateLineRenderer();

            yield return new WaitForSeconds(sampleSpeedSec);
        }
    }
    #endregion  

    #region Input Action Callbacks
    private void DrawStarted(InputAction.CallbackContext callback)
    {
        drawStart = true;
    }
    private void DrawPerformed(InputAction.CallbackContext callback)
    {
        drawHeld = true;
    }
    private void DrawCanceled(InputAction.CallbackContext callback)
    {
        drawHeld = false;
    }
    #endregion
}