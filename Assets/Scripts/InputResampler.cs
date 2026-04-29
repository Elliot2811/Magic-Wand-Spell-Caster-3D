using UnityEngine;
using System.Collections.Generic;

public class InputResampler
{
    #region Variables

    #region Instantiated Variables
    private Vector2[] _originalPoints;
    private int _numberOfPoints = 0;
    private float _distanceBetweenPoints = 0;
    #endregion

    #region Non-Instantiated Variables
    private float _totalDistance = 0;
    private float _width = 0;
    private float _height = 0;

    private Vector2[] resampledPoints;
    #endregion

    #endregion

    #region Constructors
    public InputResampler(Vector2[] timeBasedSampledPoints)
    {
        //_originalPoints = new Vector2[timeBasedSampledPoints.Length];
        //Array.Copy(timeBasedSampledPoints, _originalPoints, timeBasedSampledPoints.Length);

        _originalPoints = (Vector2[])timeBasedSampledPoints.Clone();
    }

    public InputResampler(Queue<Vector2> timeBasedSampledpoints)
    {
        _originalPoints = timeBasedSampledpoints.ToArray();
    }

    public InputResampler(Vector2[] timeBasedSampledPoints, int numPointsRecaculated)
    {
        _originalPoints = (Vector2[])timeBasedSampledPoints.Clone();
        _numberOfPoints = numPointsRecaculated;
    }

    public InputResampler(Queue<Vector2> TimeBasedSampledpoints, int numPointsRecaculated)
    {
        _originalPoints = TimeBasedSampledpoints.ToArray();
        _numberOfPoints = numPointsRecaculated;
    }

    //public InputResampler(Queue<Vector2> TimeBasedSampledpoints, float distanceBetweenPointsRecaculated)
    //{
    //    originalPoints = new Queue<Vector2>(TimeBasedSampledpoints);
    //    distanceBetweenPoints = distanceBetweenPointsRecaculated;
    //}
    #endregion

    #region Getter Functions
    public int numberOfPoints
    {
        get { return _numberOfPoints; }
        set { _numberOfPoints = value; }
    }
    public float distanceBetweenPoints
    {
        get { return _distanceBetweenPoints; }
    }
    public float width
    {
        get { return _width; }
    }
    public float height
    {
        get { return _height; }
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


    #region Normal Functions
    public void ResampleData()
    {
        if (_originalPoints == null || _originalPoints.Length <= 1)
        {
            return;
        }

        if (_totalDistance == 0)
        {
            CaculateTotalDistance();
        }

        if (numberOfPoints == 0)
        {
            return;
        }

        _distanceBetweenPoints = _totalDistance / (_numberOfPoints - 1);

        Debug.Log("Total Distance: " + _totalDistance);
        Debug.Log("Distance Between Points: " + _distanceBetweenPoints);

        ConvertToDistanceBased();
    }
    private void CaculateTotalDistance()
    {
        Vector2 previousPoint = _originalPoints[0];
        foreach (Vector2 point in _originalPoints)
        {
            _totalDistance += Vector2.Distance(previousPoint, point);
            previousPoint = point;
        }
    }

    private void ConvertToDistanceBased()
    {
        resampledPoints = new Vector2[_numberOfPoints];
        resampledPoints[0] = _originalPoints[0];

        float overflowDistance = 0;
        int counter = 1;

        for (int i = 0; i < _originalPoints.Length - 1; i++)
        {
            Vector2 pointA = _originalPoints[i];
            Vector2 pointB = _originalPoints[i + 1];

            float distPointAToB = Vector2.Distance(pointA, pointB);

            while (distPointAToB + overflowDistance >= _distanceBetweenPoints)
            {
                if (counter >= resampledPoints.Length)
                {
                    Debug.LogWarning("Reached the maximum number of points: " + _numberOfPoints);
                    return;
                }

                float distanceToNextPoint = _distanceBetweenPoints - overflowDistance;

                pointA = FindPosAlongLine(pointA, pointB, distanceToNextPoint);
                
                resampledPoints[counter++] = pointA;
                //Debug.Log("Adding Point: " + pointA);

                overflowDistance = 0f;

                distPointAToB = Vector2.Distance(pointA, pointB);
            }

            overflowDistance += distPointAToB;
        }

        if (resampledPoints[_numberOfPoints - 1] == Vector2.zero)
        {
            resampledPoints[_numberOfPoints - 1] = _originalPoints[_originalPoints.Length - 1];
        }
    }

    private Vector2 FindPosAlongLine(Vector2 start, Vector2 end, float distanceAlongLine)
    {
        Vector2 direction = (end - start).normalized;
        return (start + direction * distanceAlongLine);
    }

    public void NormalizePoints(ref List<Vector2> points)
    {
        Vector2 firstPoint = points[0];

        float minX = firstPoint.x;
        float maxX = firstPoint.x;
        float minY = firstPoint.y;
        float maxY = firstPoint.y;

        for (int i = 1; i < points.Count; i++)
        {
            Vector2 point = points[i];
            if (point.x < minX) minX = point.x;
            if (point.x > maxX) maxX = point.x;
            if (point.y < minY) minY = point.y;
            if (point.y > maxY) maxY = point.y;
        }

        _width = maxX - minX;
        _height = maxY - minY;

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 point = points[i];
            float normalizedX = (point.x - minX) / _width;
            float normalizedY = (point.y - minY) / _height;
            points[i] = new Vector2(normalizedX, normalizedY);
        }

        //Debug.Log("Normalized Points:");

        //foreach (Vector2 point in points)
        //{
        //    Debug.Log(point);
        //}
    }


    public void PrintResampledPoints()
    {
        Debug.Log("Printing Resampled Points");

        foreach (var point in resampledPoints)
        {
            Debug.Log(point);
        }
    }
    #endregion
}