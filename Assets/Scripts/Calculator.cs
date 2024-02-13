using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Calculator : MonoBehaviour
{

    [SerializeField] private TMP_InputField exportText;
    [SerializeField] private InputField importText;
    [SerializeField] private RectTransform contentView;
    [SerializeField] private GameObject exportPane;
    [SerializeField] private GameObject importPane;
    private bool generate;
    public static bool exportPaneActive, importPaneActive;

    private List<string> 
        type_drive = new(){
            "forward", "backward", "left", "right",
            "forwardRight", "forwardLeft", "backwardLeft", "backwardRight"},
        type_turn = new(){
            "turnLeft", "turnRight"},
        type_elbow = new(){
            "flipElbow", "restElbow"},
        type_claw = new(){
            "openLeftClaw", "openRightClaw", "openBothClaws",
            "closeLeftClaw", "closeRightClaw", "closeBothClaws"},
        type_arm = new(){
            "raiseArm", "lowerArm"};


    void Start()
    {
        exportPane.SetActive(false);
        importPane.SetActive(false);
        exportPaneActive = false;
        importPaneActive = false;
        exportText.text = "";
        importText.text = "";
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

        // trigger export pane
        if (Input.GetKeyDown(KeyCode.E) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) && !importPaneActive)
        {
            exportPaneActive = !exportPaneActive;
            importPaneActive = false;
            generate = true;
        }

        // display copyable text onscreen
        if (generate)
        {
            exportText.text = GenerateString();
            generate = false;
        }

        // trigger import pane
        if (Input.GetKeyDown(KeyCode.I) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)))
        {
            importPaneActive = true;
            exportPaneActive = false;
        }

        // adjust pane height
        int newlines = Regex.Matches(exportText.text, "\n|\r").Count;
        contentView.sizeDelta = new Vector2(contentView.sizeDelta.x, newlines * 70);
        exportPane.SetActive(exportPaneActive);
        importPane.SetActive(importPaneActive);

    }

    public void ReadStringInput(string input)
    {
        string s = input;
        importPaneActive = false;
        importText.text = string.Empty;
        importPane.SetActive(importPaneActive);

        List<double> readX = new(), readY = new(), readH = new(), m_args = new();
        List<string> m_name = new(), m_action = new(), readA = new(), NAME = new();
        List<int> m_argsCount = new();

        int i = 0;

        GetStartPose(ref i, s, ref readX, ref readY, ref readH, ref readA, ref NAME);
        GetMovements(ref i, s, ref m_name, ref m_args, ref m_argsCount, ref m_action, ref NAME);
        GetTranslatedPoses(ref readX, ref readY, ref readH, ref readA, ref m_name, ref m_action, ref m_argsCount, ref m_args);
        ReplaceOldLists(ref readX, ref readY, ref readH, ref readA);

    }

    private (string findString, string findStringComp) FindNext(ref int i, char c, string s)
    {
        string findString = "";
        while (i < s.Length && s[i] != c)
        {
            findString += s[i];
            i++;
        }
        int j = i;
        string findStringComp = "";
        while (j < s.Length)
        {
            j++;
            if (j < s.Length)
                findStringComp += s[j];
        }

        return (findString, findStringComp);
    }

    private void MatchAction(string s, string findString, ref List<string> m_action, ref List<string> NAME)
    {
        if (type_drive.Contains(s))
        {
            m_action.Add("DRIVE");
            NAME.Add("");
            return;
        }
        if (type_turn.Contains(s))
        {
            m_action.Add(s.Equals("turnLeft") ? "TURN_LEFT" : "TURN_RIGHT");
            NAME.Add(findString);
            return;
        }
        if (type_arm.Contains(s))
        {
            m_action.Add("ARM");
            NAME.Add(findString);
            return;
        }
        if (type_elbow.Contains(s))
        {
            m_action.Add("ELBOW");
            NAME.Add(findString);
            return;
        }
        if (type_claw.Contains(s))
        {
            m_action.Add("CLAW");
            NAME.Add(findString);
            return;
        }
        if (s.Equals("sleepFor"))
        {
            m_action.Add("DELAY");
            NAME.Add(findString);
            return;
        }
        m_action.Add("null");
        NAME.Add("");
    }

    private void GetStartPose(ref int i, string s, ref List<double> readX, ref List<double> readY, ref List<double> readH, ref List<string> readA, ref List<string> NAME)
    {
        i = 0;
        FindNext(ref i, '(', s);

        string parse = "";
        int j = 0;
        while (s[i] != ')' && i < s.Length)
        {
            i++;
            if (i >= s.Length) break;
            if (",)".Contains(s[i]))
            {
                double val;
                if (double.TryParse(parse, out _))
                    val = double.Parse(parse);
                else
                    val = 0;

                if (j == 0)
                    readX.Add(val);
                if (j == 1)
                    readY.Add(val);
                if (j == 2)
                    readH.Add(val);

                j++;
                parse = "";
            }
            else
                parse += s[i];

        }

        double v;
        if (double.TryParse(parse, out _))
            v = double.Parse(parse);
        else
            v = 0;

        if (j == 2)
        {
            readH.Add(v);
            j++;
        }

        for (int _ = j; _ <= 2; _++)
        {
            if (_ == 0)
                readX.Add(0);
            if (_ == 1)
                readY.Add(0);
            if (_ == 2)
                readH.Add(0);
        }

        readA.Add("DRIVE");
        NAME.Add("");
    }


    private void GetMovements(ref int i, string s, ref List<string> m_name, ref List<double> m_args, ref List<int> m_argsCount, ref List<string> m_action, ref List<string> NAME)
    {
        while (i < s.Length)
        {
            FindNext(ref i, '.', s);
            (string findString, string findStringComp) find = FindNext(ref i, '(', s);
            string findString = find.findString, findStringComp = find.findStringComp;
            if (findString.Length > 0)
            {
                m_name.Add(findString);
                MatchAction(s, findString, ref m_action, ref NAME);
            }

            findString = "";
            int parenthesesCount = 1;
            while (parenthesesCount != 0 && i < s.Length)
            {
                i++;
                if (i < s.Length)
                {
                    parenthesesCount += s[i] == '(' ? 1 : (s[i] == ')' ? -1 : 0);
                    if (s[i] != ')' && parenthesesCount != 0)
                        findString += s[i];
                }
            }

            if (i < s.Length)
            {
                int i_temp = i, argsCount = 1;
                findStringComp = findString;
                while (!findStringComp.Contains("."))
                {
                    argsCount++;
                    i = 0;
                    find = FindNext(ref i, '.', findStringComp);
                    findString = find.findString; findStringComp = find.findStringComp;
                    m_args.Add(Eval(findString));
                }
                m_args.Add(Eval(findStringComp));
                m_argsCount.Add(argsCount);
                i = i_temp;
            }
        }
    }

    private void GetTranslatedPoses(ref List<double> readX, ref List<double> readY, ref List<double> readH, ref List<string> readA, ref List<string> m_name, ref List<string> m_action, ref List<int> m_argsCount, ref List<double> m_args)
    {
        double t_x = readX[0], t_y = readY[0], t_h = readH[0];
        for (int i = 0; i < m_name.Count; i++)
        {
            string t_n = m_name[0], t_a = m_action[0];
            double t_arg1 = 0, t_arg2 = 0;
            if (m_argsCount[0] >= 1)
                t_arg1 = m_args[0];
            if (m_argsCount[0] > 1)
                t_arg2 = m_args[1];
            for (int _ = 0; _ < m_argsCount[0]; _++)
                m_args.RemoveAt(0);
            m_argsCount.RemoveAt(0);
            m_name.RemoveAt(0);
            m_action.RemoveAt(0);

            double t_h_rad = t_h * Math.PI / 180.0;
            if (t_n.Equals("forward"))
            {
                t_x += t_arg1 * Math.Cos(t_h_rad);
                t_y += t_arg1 * Math.Sin(t_h_rad);
            }
            if (t_n.Equals("backward"))
            {
                t_x += t_arg1 * Math.Cos(t_h_rad + Math.PI);
                t_y += t_arg1 * Math.Sin(t_h_rad + Math.PI);
            }
            if (t_n.Equals("left"))
            {
                t_x += t_arg1 * Math.Cos(t_h_rad + Math.PI / 2);
                t_y += t_arg1 * Math.Sin(t_h_rad + Math.PI / 2);
            }
            if (t_n.Equals("right"))
            {
                t_x += t_arg1 * Math.Cos(t_h_rad + -Math.PI / 2);
                t_y += t_arg1 * Math.Sin(t_h_rad + -Math.PI / 2);
            }
            if (t_n.Equals("forwardLeft"))
            {
                t_arg2 = -t_arg2;
                t_x += t_arg2 * Math.Cos(t_h_rad + -Math.PI / 2) - t_arg1 * Math.Sin(t_h_rad + -Math.PI / 2);
                t_y += t_arg2 * Math.Sin(t_h_rad + -Math.PI / 2) + t_arg1 * Math.Cos(t_h_rad + -Math.PI / 2);
            }
            if (t_n.Equals("forwardRight"))
            {
                t_x += t_arg2 * Math.Cos(t_h_rad + -Math.PI / 2) - t_arg1 * Math.Sin(t_h_rad + -Math.PI / 2);
                t_y += t_arg2 * Math.Sin(t_h_rad + -Math.PI / 2) + t_arg1 * Math.Cos(t_h_rad + -Math.PI / 2);
            }
            if (t_n.Equals("backwardLeft"))
            {
                t_arg1 = -t_arg1;
                t_arg2 = -t_arg2;
                t_x += t_arg2 * Math.Cos(t_h_rad + -Math.PI / 2) - t_arg1 * Math.Sin(t_h_rad + -Math.PI / 2);
                t_y += t_arg2 * Math.Sin(t_h_rad + -Math.PI / 2) + t_arg1 * Math.Cos(t_h_rad + -Math.PI / 2);
            }
            if (t_n.Equals("backwardRight"))
            {
                t_arg1 = -t_arg1;
                t_x += t_arg2 * Math.Cos(t_h_rad + -Math.PI / 2) - t_arg1 * Math.Sin(t_h_rad + -Math.PI / 2);
                t_y += t_arg2 * Math.Sin(t_h_rad + -Math.PI / 2) + t_arg1 * Math.Cos(t_h_rad + -Math.PI / 2);
            }
            if (t_n.Equals("turnLeft"))
                t_h += t_arg1;
            if (t_n.Equals("turnRight"))
                t_h -= t_arg1;

            readX.Add(t_x); 
            readY.Add(t_y); 
            readH.Add(t_h); 
            readA.Add(t_a);

        }
    }

    private void ReplaceOldLists(ref List<double> readX, ref List<double> readY, ref List<double> readH, ref List<string> readA)
    {
        PJ.X.Clear();
        PJ.Y.Clear();
        PJ.HEADING.Clear();
        PJ.ACTION.Clear();

        int iterations = readX.Count;
        for (int i = 0; i < iterations; i++)
        {
            PJ.X.Add(readX[0]);
            PJ.Y.Add(readY[0]);
            PJ.HEADING.Add(readH[0]);
            PJ.ACTION.Add(readA[0]);
            readX.RemoveAt(0);
            readY.RemoveAt(0);
            readH.RemoveAt(0);
            readA.RemoveAt(0);
        }

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

    private static double Eval(string s)
    {
        return Convert.ToDouble(new DataTable().Compute(s, ""));
    }

}
