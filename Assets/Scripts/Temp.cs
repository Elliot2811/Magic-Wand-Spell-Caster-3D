using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    private Queue<Vector2> points = new Queue<Vector2>();

    public void Start()
    {
        points.Enqueue(new Vector2(0, 0));
        points.Enqueue(new Vector2(1, 0));

        PointsManipulation.PrintPoints(points.ToArray());

        Vector2[] resampledPoints = PointsManipulation.ResamplePoints(points, 10);

        PointsManipulation.PrintPoints(resampledPoints);

        Vector2[] normalizedPoints = PointsManipulation.NormalizePoints(resampledPoints);

        PointsManipulation.PrintPoints(normalizedPoints);

        Vector2[] scaledUpPoints = PointsManipulation.ScaleToScreen(normalizedPoints, Camera.main, 0.5f);

        PointsManipulation.PrintPoints(scaledUpPoints);

        scaledUpPoints = PointsManipulation.ScaleToScreen(normalizedPoints, Camera.main, 0.75f);

        PointsManipulation.PrintPoints(scaledUpPoints);
    }
}