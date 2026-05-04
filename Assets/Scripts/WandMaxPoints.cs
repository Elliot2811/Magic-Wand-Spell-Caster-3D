using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class WandMaxPoints : WandBase
{
    public int MaxPoints = 100;

    protected override void Start()
    {
        base.Start();
        
        StartCoroutine(LogMousePos());
    }

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

            yield return new WaitForSeconds(SampleSpeedSec);

            stopwatch.Stop();
            UnityEngine.Debug.Log("Time taken to log mouse position: " + stopwatch.ElapsedMilliseconds + " ms");
            stopwatch.Reset();
        }
    }
}