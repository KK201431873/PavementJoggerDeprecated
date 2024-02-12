using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Calculator : MonoBehaviour
{

    [SerializeField] private TMP_InputField text;
    [SerializeField] private RectTransform contentView;
    [SerializeField] private GameObject pane;
    private bool generate;

    void Start()
    {
        pane.SetActive(false);
        generate = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            for (int i = 0; i < PJ.X.Count; i++)
            {
                PJ.Y[i] = -PJ.Y[i];
                PJ.HEADING[i] = -PJ.HEADING[i];
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
            generate = !generate;

        // display copyable text onscreen
        if (generate)
        {
            pane.SetActive(true);
            text.text = GenerateString();
        }
        else
        {
            pane.SetActive(false);
            text.text = "";
        }

        // adjust pane height
        int newlines = Regex.Matches(text.text, "\n").Count;
        contentView.sizeDelta = new Vector2(contentView.sizeDelta.x, newlines * 60);

    }

    private string GenerateString()
    {

        if (PJ.X.Count <= 1)
            return "";

        double x = Round(PJ.X[0], 2);
        double y = Round(PJ.Y[0], 2);
        double h = Round(PJ.HEADING[0], 2);

        string res = "// START POSE ("+x+","+y+","+h+")\n";

        for (int i = 1; i < PJ.X.Count; i++)
        {
            string curFunc = "";

            double dx = Round(PJ.X[i] - x, 2);
            double dy = Round(PJ.Y[i] - y, 2);
            double dh = Round(NormalizeAngle(PJ.HEADING[i] - h), 2);

            float theta = 90f - (float)h;
            double dx_temp = Round(dx * Math.Cos(theta * Math.PI / 180f) - dy * Math.Sin(theta * Math.PI / 180f), 2);
            double dy_temp = Round(dx * Math.Sin(theta * Math.PI / 180f) + dy * Math.Cos(theta * Math.PI / 180f), 2);
            dx = dx_temp;
            dy = dy_temp;

            double abs_dx = Math.Abs(dx);
            double abs_dy = Math.Abs(dy);
            double abs_dh = Math.Abs(dh);

            // fwd
            if (dy > 0)
            {
                curFunc = ".forward";
                if (dx > 0)
                    curFunc += "Right(" + abs_dy + ", " + abs_dx + ")";
                if (dx < 0)
                    curFunc += "Left(" + abs_dy + ", " + abs_dx + ")";
            }

            // bwd
            if (dy < 0)
            {
                curFunc = ".backward";
                if (dx > 0)
                    curFunc += "Right(" + abs_dy + ", " + abs_dx + ")";
                if (dx < 0)
                    curFunc += "Left(" + abs_dy + ", " + abs_dx + ")";
            }

            // fwd/bwd pure
            if (dx == 0 && dy != 0)
                curFunc += "(" + abs_dy + ")";

            // left/right pure
            if (dy == 0)
            {
                if (dx > 0)
                    curFunc = ".right(" + abs_dx + ")";
                if (dx < 0)
                    curFunc = ".left(" + abs_dx + ")";
            }

            if (curFunc.Length > 0)
                res += curFunc + "\n";

            // turn
            if (dh > 0)
                curFunc = ".turnLeft";
            if (dh < 0)
                curFunc = ".turnRight";
            if (dh != 0)
            {
                curFunc += "(" + abs_dh + ")";
                res += curFunc + "\n";
            }

            x = Round(PJ.X[i], 2);
            y = Round(PJ.Y[i], 2);
            h = Round(PJ.HEADING[i], 2);

        }

        return res;
    }

    private double NormalizeAngle(double theta)
    {
        double th = theta;
        while (th > 180)
            th -= 360;
        while (th <= -180)
            th += 360;
        return th;
    }

    private double Round(double x)
    {
        return System.Math.Round(x);
    }

    private double Round(double x, int places)
    {
        return System.Math.Round(x, places);
    }

    private double RoundToNearest(double x, double multiple)
    {
        return System.Math.Round(x / multiple) * multiple;
    }

    private double Hypot(double x, double y)
    {
        return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
    }

}
