using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WandBase : MonoBehaviour
{
    #region Global Varaibles

    #region Component References

    public LineRenderer lineRenderer;
    protected WandInputActions inputActions;

    #endregion

    #region Inspector Variables
    public float SampleSpeedSec = 0.01f;

    public float LineWidth = 0.1f;

    public Transform WandTip;
    #endregion

    #region Runtime Variables
    protected bool ortographicView = true;

    protected bool drawHeld;
    protected bool drawStart;

    protected Queue<Vector2> points = new Queue<Vector2>();
    #endregion

    #endregion

    #region MonoBehaviour Functions
    private void Awake()
    {
        inputActions = new WandInputActions();
    }

    protected virtual void Start()
    {
        lineRenderer.startWidth = LineWidth;
        lineRenderer.endWidth = LineWidth;

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

    #region Other Functions
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
        //Vector2 mousePosition = inputActions.Wand.Position.ReadValue<Vector2>();
        ////Debug.Log("Mouse Position: " + mousePosition);

        //points.Enqueue(ortographicView ? WorldPosOrthographic(mousePosition) : WorldPosPerspective(mousePosition));
        if (WandTip != null)
        {
            points.Enqueue(WandTip.position);
        }
    }

    protected Vector3 WorldPosOrthographic(Vector2 mousePos)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y));
        return worldPos;
    }

    protected Vector3 WorldPosPerspective(Vector2 mousePos)
    {
        return new Vector3(0f, 0f, 0f);
    }

    protected virtual IEnumerator LogMousePos()
    {
        while (drawHeld)
        {
            FindInsertWorldPos();
            UpdateLineRenderer();

            yield return new WaitForSeconds(SampleSpeedSec);
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