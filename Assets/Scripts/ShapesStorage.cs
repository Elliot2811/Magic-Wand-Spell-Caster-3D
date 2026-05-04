using UnityEngine;

// Change editor script if add variables seen in inspector
public class ShapesStorage : MonoBehaviour
{
    [SerializeField]
    private int numberOfShapes = 0;

    [SerializeField]
    private ShapeInformation[] shapes;

    private Vector2[][][] shapesData;
    private Vector2[][][] resampledShapesData;
    private int numPointsResampledSet = 0;

    private string[] shapesNames;
    public string[] getShapesNames
    {
        get { return shapesNames; }
    }

    private void Start()
    {
        shapesData = new Vector2[numberOfShapes][][];
        shapesNames = new string[numberOfShapes];

        for (int shapeIndex = 0; shapeIndex < numberOfShapes; shapeIndex++)
        {
            if (shapes[shapeIndex] == null)
                continue;
            
            shapesData[shapeIndex] = shapes[shapeIndex].GetShapeData();
            shapesNames[shapeIndex] = shapes[shapeIndex].GetShapeName;

            Debug.Log($"Shape {shapes[shapeIndex]}");
        }
    }


    public Vector2[][][] RequestShapesData()
    {
        if (shapesData == null || shapesData.Length == 0)
            return new Vector2[0][][];

        return shapesData;
    }

    public Vector2[][][] RequestShapesData(bool dataIsResampled, int numPoints)
    {
        if (shapesData == null || shapesData.Length == 0)
            return new Vector2[0][][];

        if (!dataIsResampled)
            return shapesData;

        if (numPoints == 0)
            return new Vector2[0][][];

        if (numPoints == numPointsResampledSet && resampledShapesData != null)
            return resampledShapesData;

        if (resampledShapesData == null)
        {
            resampledShapesData = new Vector2[numberOfShapes][][];
        }

        for (int shapeIndex = 0; shapeIndex < shapesData.Length; shapeIndex++)
        {
            if (shapesData[shapeIndex] == null) continue;

            if (resampledShapesData[shapeIndex] == null)
            {
                resampledShapesData[shapeIndex] = new Vector2[shapesData[shapeIndex].Length][];
            }

            for (int variantIndex = 0; variantIndex < shapesData[shapeIndex].Length; variantIndex++)
            {
                if (shapesData[shapeIndex][variantIndex] == null)
                    continue;

                InputResampler inputResampler = new InputResampler(shapesData[shapeIndex][variantIndex], numPoints);
                inputResampler.ResampleData();
                inputResampler.NormalizePoints();
                resampledShapesData[shapeIndex][variantIndex] = inputResampler.getPoints;
            }
        }

        numPointsResampledSet = numPoints;
        return resampledShapesData;
    }
}