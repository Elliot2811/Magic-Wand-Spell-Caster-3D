using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Global Variables

    #region Inspector Variables
    public WandBase wand;
    public ShapesStorage shapesStorage;

    public int numPointsToRecalculateTo = 100;
    public float minAccuracy = 0.6f;
    #endregion

    #region Runtime Variables
    private Vector2[] playerInputs;
    #endregion

    #endregion

    #region MonoBehaviour Functions
    private void Update()
    {
        CheckDrawing();
    }
    #endregion

    #region Normal Functions
    private void CheckDrawing()
    {
        // Request User drawn data
        if (!wand.dataReady())
        {
            return;
        }

        playerInputs = wand.RequestData(true, numPointsToRecalculateTo);

        // Compare Shapes
        CompareShapes compareShapes = new CompareShapes(
            playerInputs,
            shapesStorage.RequestShapesData(true, numPointsToRecalculateTo),
            shapesStorage.getShapesNames,
            minAccuracy
            );

        Debug.Log("Shape: " + compareShapes.ShapeType);
    }
    #endregion
}