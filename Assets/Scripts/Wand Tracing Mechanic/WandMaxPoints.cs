using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class WandMaxPoints : WandBase
{
    #region Inspector Variables
    public int MaxPoints = 100;
    #endregion

    #region Monobehvaiour Functions
    protected override void Start()
    {
        base.Start();
        
        StartCoroutine(LogMousePos());
    }
    #endregion

    #region Other Functions
    protected override IEnumerator LogMousePos()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (true)
        {
            stopwatch.Start();
            FindInsertWorldPos();

            if (points.Count > MaxPoints)
                {
                    points.Dequeue();
                }
                
            UpdateLineRenderer();

            yield return new WaitForSeconds(sampleSpeedSec);

            stopwatch.Stop();
            UnityEngine.Debug.Log("Time taken to log mouse position: " + stopwatch.ElapsedMilliseconds + " ms");
            stopwatch.Reset();
        }
    }
    #endregion
}