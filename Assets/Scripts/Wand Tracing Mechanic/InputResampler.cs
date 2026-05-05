using UnityEngine;
using System.Collections.Generic;

public class InputResampler
{
    #region Variables

    #region Instantiated Variables
    private Vector2[] originalPoints;
    private int numberOfPoints;
    private float distanceBetweenPoints = 0;
    #endregion

    #region Non-Instantiated Variables
    private float totalDistance = 0;
    private float width = 0;
    private float height = 0;

    private Vector2[] resampledPoints;
    private bool hasResampled = false;
    #endregion

    #endregion

    #region Getter Setters
    public int NumPoints
    {
        get { return numberOfPoints; }
        set { numberOfPoints = value; }
    }
    public float DistanceBetweenPoints
    {
        get { return distanceBetweenPoints; }
    }
    public float Width
    {
        get { return width; }
    }
    public float Height
    {
        get { return height; }
    }
    public Vector2[] getPoints
    {
        get
        {
            if (resampledPoints == null)
            {
                ResampleData();
            }
            return resampledPoints;
        }
    }
    #endregion

    #region Constructors
    public InputResampler(Vector2[] timeBasedSampledPoints)
    {
        //_originalPoints = new Vector2[timeBasedSampledPoints.Length];
        //Array.Copy(timeBasedSampledPoints, _originalPoints, timeBasedSampledPoints.Length);

        originalPoints = (Vector2[])timeBasedSampledPoints.Clone();
    }
    public InputResampler(Queue<Vector2> timeBasedSampledpoints)
    {
        originalPoints = timeBasedSampledpoints.ToArray();
    }
    public InputResampler(Vector2[] timeBasedSampledPoints, int numPointsRecaculated) : this(timeBasedSampledPoints)
    {
        numberOfPoints = numPointsRecaculated;
    }
    public InputResampler(Queue<Vector2> timeBasedSampledPoints, int numPointsRecaculated) : this(timeBasedSampledPoints)
    {
        numberOfPoints = numPointsRecaculated;
    }
    //public InputResampler(Queue<Vector2> TimeBasedSampledpoints, float distanceBetweenPointsRecaculated)
    //{
    //    originalPoints = new Queue<Vector2>(TimeBasedSampledpoints);
    //    distanceBetweenPoints = distanceBetweenPointsRecaculated;
    //}
    #endregion

    #region Normal Functions
    public void ResampleData()
    {
        if (hasResampled)
            return;
        hasResampled = true;

        if (originalPoints == null || originalPoints.Length <= 1)
        {
            return;
        }

        CaculateTotalDistance();

        if (numberOfPoints == 0 || totalDistance == 0)
        {
            return;
        }

        distanceBetweenPoints = totalDistance / (numberOfPoints - 1);

        if (distanceBetweenPoints <= 0)
        {
            return;
        }

        Debug.Log("Total Distance: " + totalDistance);
        Debug.Log("Distance Between Points: " + distanceBetweenPoints);

        ConvertToDistanceBased();
    }
    private void CaculateTotalDistance()
    {
        Vector2 previousPoint = originalPoints[0];
        foreach (Vector2 point in originalPoints)
        {
            totalDistance += Vector2.Distance(previousPoint, point);
            previousPoint = point;
        }
    }
    private void ConvertToDistanceBased()
    {
        resampledPoints = new Vector2[numberOfPoints];
        resampledPoints[0] = originalPoints[0];

        float overflowDistance = 0;
        int counter = 1;

        for (int i = 0; i < originalPoints.Length - 1; i++)
        {
            Vector2 pointA = originalPoints[i];
            Vector2 pointB = originalPoints[i + 1];

            float distPointAToB = Vector2.Distance(pointA, pointB);

            while (distPointAToB + overflowDistance >= distanceBetweenPoints)
            {
                if (counter >= resampledPoints.Length)
                {
                    Debug.LogWarning("Reached the maximum number of points: " + numberOfPoints);
                    return;
                }

                float distanceToNextPoint = distanceBetweenPoints - overflowDistance;

                pointA = FindPosAlongLine(pointA, pointB, distanceToNextPoint);
                
                resampledPoints[counter++] = pointA;
                //Debug.Log("Adding Point: " + pointA);

                overflowDistance = 0f;

                distPointAToB = Vector2.Distance(pointA, pointB);
            }

            overflowDistance += distPointAToB;
        }

        if (resampledPoints[numberOfPoints - 1] == Vector2.zero)
        {
            resampledPoints[numberOfPoints - 1] = originalPoints[originalPoints.Length - 1];
        }
    }

    private Vector2 FindPosAlongLine(Vector2 start, Vector2 end, float distanceAlongLine)
    {
        Vector2 direction = (end - start).normalized;
        return (start + direction * distanceAlongLine);
    }

    public void NormalizePoints()
    {
        if (resampledPoints == null || resampledPoints.Length <= 0)
            return;

        Vector2 firstPoint = resampledPoints[0];

        float minX = firstPoint.x;
        float maxX = firstPoint.x;
        float minY = firstPoint.y;
        float maxY = firstPoint.y;

        for (int i = 1; i < resampledPoints.Length; i++)
        {
            Vector2 point = resampledPoints[i];
            if (point.x < minX) minX = point.x;
            if (point.x > maxX) maxX = point.x;
            if (point.y < minY) minY = point.y;
            if (point.y > maxY) maxY = point.y;
        }

        width = maxX - minX;
        height = maxY - minY;

        // Use the larger dimension to preserve aspect ratio
        float scale = Mathf.Max(width, height);

        // Shape is a single point, nothing to normalize
        if (scale == 0) return;

        for (int i = 0; i < resampledPoints.Length; i++)
        {
            Vector2 point = resampledPoints[i];
            float normalizedX = (point.x - minX) / scale;
            float normalizedY = (point.y - minY) / scale;
            resampledPoints[i] = new Vector2(normalizedX, normalizedY);
        }
    }


    public void PrintResampledPoints()
    {
        Debug.Log("Printing Resampled Points");

        if (resampledPoints == null)
            return;

        string temp = "";

        for (int i = 0; i < resampledPoints.Length; i++)
        {
            if (i % 5 == 0)
            {
                Debug.Log(temp);
                temp = "";
            }

            temp += $"{resampledPoints[i]}, ";
        }

        Debug.Log(temp);
    }
    #endregion
}