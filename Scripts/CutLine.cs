using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutLine : MonoBehaviour
{
    public Transform[] pathPoints;
    public LineRenderer lineRenderer;

    public void SetupLine()
    {
        Debug.Log("CutLine: SetupLine() 실행됨");

        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer를 찾지 못했어요!");
            return;
        }

        lineRenderer.positionCount = pathPoints.Length;
        for (int i = 0; i < pathPoints.Length; i++)
        {
            lineRenderer.SetPosition(i, pathPoints[i].position);
            Debug.Log($" 포인트 {i}: {pathPoints[i].position}");
        }

        gameObject.SetActive(true);
    }

    public void ShowLine() => gameObject.SetActive(true);
    public void HideLine() => gameObject.SetActive(false);
}
