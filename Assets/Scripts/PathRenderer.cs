using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRenderer : MonoBehaviour
{

    [SerializeField] private LineRenderer pathRenderer;
    private int index = 0;

    public PathRenderer SetIndex(int index)
    {
        this.index = index;
        name = "Path " + index;
        return this;
    }

    public int GetIndex() { return index; }

    public void Kill()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        pathRenderer.startWidth = pathRenderer.endWidth = 0.12f;
    }

    void Update()
    {
        if (!PJ.ACTION[index].Equals("DRIVE")) return;

        var X = PJ.X;
        var Y = PJ.Y;
        var ipp = PJ.in_per_px;

        pathRenderer.positionCount = 2;
        pathRenderer.numCapVertices = pathRenderer.numCornerVertices = 90;

        // connect to tool
        if (index == 0)
        {
            pathRenderer.startColor = pathRenderer.endColor = new Color(0.5f, 0.5f, 0.5f, Tool.size.x - 1.0f);
            pathRenderer.SetPosition(0, new Vector3((float)(X[^1] / ipp), (float)(Y[^1] / ipp), 0));
            pathRenderer.SetPosition(1, new Vector3((float)Tool.x, (float)Tool.y, 0));
            return;
        }

        // connect from previous node
        pathRenderer.startColor = pathRenderer.endColor = new Color(1, 0, 0, 0.75f);
        pathRenderer.SetPosition(0, new Vector3((float)(X[index-1] / ipp), (float)(Y[index-1] / ipp), 0));
        pathRenderer.SetPosition(1, new Vector3((float)(X[index] / ipp), (float)(Y[index] / ipp), 0));
        return;

    }

    private double Hypot(double x, double y)
    {
        return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
    }

    private int Max(int x, int y)
    {
        return Math.Max(x, y);
    }

}
