public class WandClickRefresh : WandBase
{
    private bool caculatedShape = true;

    #region Monobehvaiour Functions
    protected override void Update()
    {
        if (drawStart)
        {
            drawStart = false;
            caculatedShape = false;
            
            points.Clear();
            lineRenderer.positionCount = 0;

            StartCoroutine(LogMousePos());
        }

        if (!caculatedShape && !drawHeld)
        {
            caculatedShape = true;

            CheckDrawingShape checkDrawingShape = new CheckDrawingShape(points, 10);
            checkDrawingShape.CaculateShape();
        }
    }
    #endregion
}