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


        


        //double totalDist = 0;

        //pathRenderer.positionCount = X.Count == 0 ? 2 : X.Count + 1;

        //double lastX = 0, lastY = 0;
        //for (int i = 1; i < X.Count; i++)
        //{
        //    LineRenderer lr = new();
        //    lr.numCapVertices = lr.numCornerVertices = 90;
        //    lr.startColor = lr.endColor = new Color(1, 0, 0, 0.75f);
        //    lr.positionCount = 2;

        //    pathRenderer.SetPosition(i, new Vector3((float)(X[i] / ipp), (float)(Y[i] / ipp), 0));
        //    lastX = X[Max(0, i - 1)] / ipp;
        //    lastY = Y[Max(0, i - 1)] / ipp;
        //    lr.SetPosition(0, new Vector3((float)lastX, (float)lastY, 0));
        //    lr.SetPosition(1, new Vector3((float)(X[i] / ipp), (float)(Y[i] / ipp), 0));
        //    totalDist += Hypot(X[i] / ipp - lastX, Y[i] / ipp - lastY);
        //}
        //var tx = (float)Tool.x;
        //var ty = (float)Tool.y;
        //double divisor = totalDist + Hypot(tx - lastX, ty - lastY);
        //totalDist /= divisor != 0 ? divisor : 1;

        //if (X.Count == 0)
        //    pathRenderer.SetPosition(0, Vector3.zero);


        //Gradient gradient = new();
        //gradient.SetKeys(
        //    new GradientColorKey[] { new(Color.red, (float)totalDist - 1E-3f), new(Color.red, (float)totalDist), new(Color.gray, (float)totalDist + 1E-3f) },
        //    new GradientAlphaKey[] { new(0.75f, (float)totalDist - 1E-3f), new(0.75f, (float)totalDist), new(Tool.size.x - 1.0f, (float)totalDist + 1E-3f) }
        //);
        //gradient.mode = GradientMode.Fixed;


        //LineRenderer lineRenderer = new();
        //lineRenderer.numCapVertices = lineRenderer.numCornerVertices = 90;
        //lineRenderer.startColor = lineRenderer.endColor = new Color(0.5f, 0.5f, 0.5f, Tool.size.x - 1.0f);
        //lineRenderer.positionCount = 2;
        //lineRenderer.SetPosition(0, new Vector3((float)lastX, (float)lastY, 0));
        //lineRenderer.SetPosition(1, new Vector3((float)Tool.x, (float)Tool.y, 0));

        //pathRenderer.startColor = new Color(1, 0, 0, 0.75f);
        //pathRenderer.colorGradient = gradient;
        //pathRenderer.endColor = new Color(0.5f, 0.5f, 0.5f, Tool.size.x - 1.0f);
        //pathRenderer.SetPosition(pathRenderer.positionCount - 1, new Vector3((float)Tool.x, (float)Tool.y, 0));

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
