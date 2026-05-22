using UnityEngine.InputSystem;

public class WandMaxPoints : WandBase
{
    public int MaxPoints = 100;

    protected override void Start()
    {
        base.Start();

        drawActive = true;
        loggingCoroutine = StartCoroutine(LogMousePos());
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (loggingCoroutine != null)
            StopCoroutine(loggingCoroutine);
    }

    protected override void OnPointRecorded()
    {
        base.OnPointRecorded();

        while (points.Count > MaxPoints)
        {
            points.Dequeue();
        }
    }

    protected override void DrawCanceled(InputAction.CallbackContext callback)
    {
        // Ensure drawActive is not deactivated.
    }
}