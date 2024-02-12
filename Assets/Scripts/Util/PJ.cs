
using System.Collections.Generic;

public static class PJ
{

    public static List<double> X = new(), Y = new(), HEADING = new(), ARM = new();
    public static List<string> ACTION = new();
    public static List<long> DELAY = new();

    public static string MODE = "CARDINAL";
    public static double in_per_px = 72.0 / 5.0;
    public static double precision = 3.0d;
    public static List<(string req, int index, (double x, double y, double heading, double arm, string action, long delay) values)> requests = new();

    public static void Add((double x, double y, double heading, double arm, string action, long delay) values)
    {
        Add(X.Count, values);
    }

    public static void Add(int index, (double x, double y, double heading, double arm, string action, long delay) values)
    {
        requests.Add(("ADD", index, values));
    }

    public static void Replace(int index, (double x, double y, double heading, double arm, string action, long delay) values)
    {
        requests.Add(("REPLACE", index, values));
    }

    public static void Remove(int index)
    {
        requests.Add(("REMOVE", index, (0, 0, 0, 0, "", 0)));
    }

    public static void ClearFrom(int index)
    {
        requests.Add(("CLEAR", index, (0, 0, 0, 0, "", 0)));
    }
}
