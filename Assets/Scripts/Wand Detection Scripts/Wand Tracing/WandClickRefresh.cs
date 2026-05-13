using UnityEngine.InputSystem;

public class WandClickRefresh : WandBase
{
    protected override void DrawStarted(InputAction.CallbackContext callback)
    {
        base.DrawStarted(callback);
        
        points.Clear();

        loggingCoroutine = StartCoroutine(LogMousePos());
    }

    protected override void DrawCanceled(InputAction.CallbackContext callback)
    {
        if (loggingCoroutine != null)
            StopCoroutine(loggingCoroutine);

        base.DrawCanceled(callback);
    }
}