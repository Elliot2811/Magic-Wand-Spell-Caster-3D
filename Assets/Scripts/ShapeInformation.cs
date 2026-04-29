using UnityEngine;

// Change editor script if add variables seen in inspector
public class ShapeInformation : MonoBehaviour
{
    //[Header("Shape information")]
    [SerializeField]
    private string shapeName;
    [SerializeField]
    private int numberOfVariants = 0;

    [SerializeField]
    private ShapeVariantInformation[] variants;

    //private int realNumOfVariants = 0;

    public string getShapeName
    {
        get { return shapeName; }
    }

    public Vector2[][] getShapeData
    {
        get
        {
            Vector2[][] shapeData = new Vector2[numberOfVariants][];

            for (int i = 0; i < numberOfVariants; i++)
            {
                shapeData[i] = variants[i].getPoints;
            }

            return shapeData;
        }
    }

    //private void updateRealNumOfVariants()
    //{
    //    foreach (var variant in variants)
    //    {
    //        if (variant.validVariant())
    //        {
    //            realNumOfVariants++;
    //        }
    //    }
    //} 
}