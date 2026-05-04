using UnityEngine;

public class WandClickRefresh : WandBase
{
    private bool newData = false;
    private bool dataConsumed = true;

    #region Monobehvaiour Functions
    protected override void Update()
    {
        if (drawStart)
        {
            drawStart = false;
            newData = false;
            dataConsumed = false;
            
            points.Clear();
            lineRenderer.positionCount = 0;

            StartCoroutine(LogMousePos());
        }

        if (!newData && !drawHeld && !dataConsumed)
        {
            newData = true;
        }
    }
    #endregion

    #region Normal Functions
    public override bool dataReady()
    {
        return newData;
    }

    public override Vector2[] RequestData()
    {
        return RequestData(false, 0);
    }

    public override Vector2[] RequestData(bool dataIsResampled, int numPoints)
    {
        if (!newData)
        {
            return new Vector2[0];
        }

        newData = false;
        dataConsumed = true;

        if (dataIsResampled)
        {
            InputResampler inputResampler = new InputResampler(points, numPoints);
            inputResampler.ResampleData();
            inputResampler.NormalizePoints();

            return inputResampler.getPoints;
        }

        return points.ToArray();

    }
    #endregion
}