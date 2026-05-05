using UnityEngine;

// Change editor script if add variables seen in inspector
public class ShapeVariantInformation : MonoBehaviour
{
    [SerializeField]
    private int numberOfPoints;

    [SerializeField]
    private Vector2[] points;

    public Vector2[] getPoints()
    {
        return this.getPoints(false, 0);
    }

    public Vector2[] getPoints(bool dataIsResampled, int numPoints)
    {
        if (dataIsResampled)
        {
            InputResampler inputResampler = new InputResampler(points, numPoints);
            inputResampler.ResampleData();
            inputResampler.NormalizePoints();

            numberOfPoints = numPoints;
            points = inputResampler.getPoints;
        }

        return points;
    }

    //public bool validVariant() // May want to add more to this function such as check 
    //{
    //    foreach (Vector2 point in points)
    //    {
    //        if (point != Vector2.zero) return true;
    //    }

    //    return false;
    //}
}