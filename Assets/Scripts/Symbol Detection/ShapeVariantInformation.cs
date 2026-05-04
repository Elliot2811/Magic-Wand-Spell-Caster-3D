using UnityEngine;

// Change editor script if add variables seen in inspector
public class ShapeVariantInformation : MonoBehaviour
{
    [SerializeField]
    private int numberOfPoints;

    [SerializeField]
    private Vector2[] points;

    public Vector2[] getPoints
    {
        get { return points; }
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