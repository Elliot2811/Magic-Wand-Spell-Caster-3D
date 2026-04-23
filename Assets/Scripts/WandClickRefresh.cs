using System.Collections;

public class WandClickRefresh : WandBasic
{
    protected virtual void Update()
    {
        if (drawStart)
        {
            drawStart = false;
            
            points.Clear();
            lineRenderer.positionCount = 0;

            StartCoroutine(LogMousePos());
        }

        if (points.Count == lineRenderer.positionCount)
        {
            return;
        }
        UpdateLineRenderer();
    }
}