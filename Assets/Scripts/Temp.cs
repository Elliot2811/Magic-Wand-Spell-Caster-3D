using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    private Queue<Vector2> points = new Queue<Vector2>();

    public void Start()
    {
        points.Enqueue(new Vector2(0, 0));
        points.Enqueue(new Vector2(1, 0));
        points.Enqueue(new Vector2(1, 1));
        points.Enqueue(new Vector2(0, 1));
        points.Enqueue(new Vector2(0, 0));

        Vector2[] newPoints = PointsManipulation.ResampleAndNormalize(points, 9);
        PointsManipulation.PrintPoints(newPoints);
    }
}