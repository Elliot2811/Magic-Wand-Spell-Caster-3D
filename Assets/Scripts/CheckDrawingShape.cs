using UnityEngine;
using System.Collections.Generic;
using UnityEditor.MPE;

public class CheckDrawingShape
{
    #region Variables

    #region Instantiated Variables
    private List<Vector2> _originalPoints;
    private int _numberOfPoints = 0;
    private float _distanceBetweenPoints = 0;
    #endregion

    #region Non-Instantiated Variables
    private float _totalDistance = 0;
    private float _width = 0;
    private float _height = 0;

    private Vector2[] _distanceBasedPoints;
    private Shape _shape = Shape.Uncaculated;
    #endregion

    #endregion

    #region Enums
    public enum Shape
    {
        Uncaculated = 0,
        Unknown = 1,
        Line,
        Circle,
        Square,
        Triangle,
    }
    #endregion

    #region Constructors
    public CheckDrawingShape(Queue<Vector2> TimeBasedSampledpoints, int numPointsRecaculated)
    {
        _originalPoints = new List<Vector2>(TimeBasedSampledpoints);
        _numberOfPoints = numPointsRecaculated;
    }

    //public CheckDrawingShape(Queue<Vector2> TimeBasedSampledpoints, float distanceBetweenPointsRecaculated)
    //{
    //    originalPoints = new Queue<Vector2>(TimeBasedSampledpoints);
    //    distanceBetweenPoints = distanceBetweenPointsRecaculated;
    //}
    #endregion

    #region Getter Functions
    public int numberOfPoints
    {
        get { return _numberOfPoints; }
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
    public Shape shape
    {
        get
        {
            if (_shape == Shape.Uncaculated)
            {
                CaculateShape();
            }

            return shape;
        }
    }
    #endregion


    #region Normal Functions

    public void CaculateShape()
    {
        if (_originalPoints == null || _originalPoints.Count <= 1)
        {
            return;
        }

        CaculateTotalDistance();

        if (_distanceBetweenPoints == 0)
        {
            _distanceBetweenPoints = _totalDistance / (_numberOfPoints - 1);
        }

        Debug.Log("Total Distance: " + _totalDistance);
        Debug.Log("Distance Between Points: " + _distanceBetweenPoints);

        ConvertTimeBasedToDistanceBased();
    }

    private void ConvertTimeBasedToDistanceBased()
    {
        _distanceBasedPoints = new Vector2[_numberOfPoints];
        _distanceBasedPoints[0] = _originalPoints[0];

        float overflowDistance = 0;
        int counter = 1;

        for (int i = 0; i < _originalPoints.Count - 1; i++)
        {
            Vector2 pointA = _originalPoints[i];
            Vector2 pointB = _originalPoints[i + 1];

            float distPointAToB = Vector2.Distance(pointA, pointB);

            while (distPointAToB + overflowDistance >= _distanceBetweenPoints)
            {
                if (counter >= _distanceBasedPoints.Length)
                {
                    Debug.LogError("Reached the maximum number of points: " + _numberOfPoints);
                    return;
                }

                float distanceToNextPoint = _distanceBetweenPoints - overflowDistance;

                pointA = FindPosAlongLine(pointA, pointB, distanceToNextPoint);
                
                _distanceBasedPoints[counter++] = pointA;
                //Debug.Log("Adding Point: " + pointA);

                overflowDistance = 0f;

                distPointAToB = Vector2.Distance(pointA, pointB);
            }

            overflowDistance += distPointAToB;
        }

        if (_distanceBasedPoints[_numberOfPoints - 1] == Vector2.zero)
        {
            _distanceBasedPoints[_numberOfPoints - 1] = _originalPoints[_originalPoints.Count - 1];
        }
    }

    private Vector2 FindPosAlongLine(Vector2 start, Vector2 end, float distanceAlongLine)
    {
        Vector2 direction = (end - start).normalized;
        return (start + direction * distanceAlongLine);
    }

    private void NormalizePoints(ref List<Vector2> points)
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

        Debug.Log("Normalized Points:");

        foreach (Vector2 point in points)
        {
            Debug.Log(point);
        }
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
    #endregion
}