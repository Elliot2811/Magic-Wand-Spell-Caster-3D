using UnityEngine;

public class WandClickRefresh : WandBase
{
    private bool resampledPoints = true;

    #region Monobehvaiour Functions
    protected override void Update()
    {
        if (drawStart)
        {
            drawStart = false;
            resampledPoints = false;
            
            points.Clear();
            lineRenderer.positionCount = 0;

            StartCoroutine(LogMousePos());
        }

        if (!resampledPoints && !drawHeld)
        {
            Debug.Log("Starting resampling");

            resampledPoints = true;

            InputResampler inputResampler = new InputResampler(points, 10);
            inputResampler.ResampleData();
            inputResampler.PrintResampledPoints();
        }
    }
    #endregion
}