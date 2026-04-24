public class WandClickRefresh : WandBase
{
    #region Monobehvaiour Functions
    protected override void Update()
    {
        if (drawStart)
        {
            drawStart = false;
            
            points.Clear();
            lineRenderer.positionCount = 0;

            StartCoroutine(LogMousePos());
        }
    }
    #endregion
}