using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.VersionControl.Asset;

public class MainMenuState : GameState
{
    GameStateManager gameManager;
    // Left Wand
    private Wand wandLeft;
    private Vector2 cursorLeftScreenPos;

    // Right Wand
    private Wand wandRight;
    private Vector2 cursorRightScreenPos;

    // Maps
    private int numMaps;
    private List<(Image mapImage, MapData mapData)> imageMapList;

    private List<Vector2> resampledPoints;

    protected override AudioPair Music => stateManager?.audioLibrary?.mapSelectionMusic;


    public override void EnterState(GameStateManager gameStateManager)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
            SceneManager.LoadScene("MainMenu");
        gameManager = gameStateManager;
        wandLeft = gameManager.wandLeft;
        wandRight = gameManager.wandRight;
        gameManager.wandListenerLeft.ChangeShapesCollection(gameManager.triangleOnlyShapeCollection);
        gameManager.wandListenerRight.ChangeShapesCollection(gameManager.triangleOnlyShapeCollection);

        imageMapList = GameStateManager.Instance.imageAndMapList;
        Debug.Log(imageMapList[0]);
        Debug.Log(imageMapList[1]);
        //numMaps = GameConstants.Instance.mapPresets.Length;
        //for (int i = 0; i < numMaps; i++)
        //{
        //    imageMapList.Add((GameStateManager.Instance.imageList[i], GameConstants.Instance.mapPresets[i]));
        //    Debug.Log(imageMapList[i]);
        //}
    }
    public override void UpdateState()
    {
        cursorLeftScreenPos = wandLeft.CurrentScreenPos;
        cursorRightScreenPos = wandRight.CurrentScreenPos;
    }

    //private void SubscribeWand()
    //{
    //    wandLeft.OnDrawingComplete += HandleLeftWandDraw;
    //    wandRight.OnDrawingComplete += HandleRightWandDraw;
    //}

    //private void HandleLeftWandDraw(Vector2[] points)
    //{
    //    CheckPointsInsideMap(points, lineRendererLeft, true);
    //}

    //private void HandleRightWandDraw(Vector2[] points)
    //{
    //    CheckPointsInsideMap(points, lineRendererRight, false);
    //}

    //private void CheckPointsInsideMap(Vector2[] points, LineRenderer lineRenderer,bool isDrawingLeft)
    //{
    //    Vector2[] processedPoints = PointsManipulation.ResampleAndNormalize(points, GameConstants.PointCount);
    //    ShapeInfoSO shapeInfo = CompareShapes.FindBestMatch(processedPoints, gameManager.triangleOnlyShapeCollection);
    //    DisplayBestShape(shapeInfo, processedPoints, lineRenderer, isDrawingLeft);
    //}

    //private void DisplayBestShape(ShapeInfoSO shapeInfo, Vector2[] points, LineRenderer lineRenderer, bool isDrawingLeft)
    //{
    //    if (shapeInfo == null)
    //        return;

    //    Vector2[] shapeData = shapeInfo.RandomVariantData;
    //    if (shapeData == null || shapeData.Length <= 1)
    //    {
    //        lineRenderer.positionCount = 0;
    //        return;
    //    }

    //    Vector2[] catmulledData = PointsManipulation.CatmullRomLine(shapeData);
    //    Vector3[] rescaledData = PointsManipulation.ScaleToViewPort(
    //        catmulledData,
    //        Camera.main,
    //        10,
    //        isDrawingLeft ? GameConstants.DrawingRectLeft : GameConstants.DrawingRectRight,
    //        GameConstants.DisplayShapePercentage
    //        );

    //    LineRendererInterface.Points(lineRenderer, rescaledData);
    //}

    public override void ExitState()
    {
        base.ExitState();
    }
}
