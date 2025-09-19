using System;
using UnityEngine;

public class Line : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float offsetSpeed = 0.1f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        var offset = lineRenderer.material.mainTextureOffset;
        offset.x += offsetSpeed * Time.deltaTime;
        lineRenderer.material.mainTextureOffset = offset;
    }
}
