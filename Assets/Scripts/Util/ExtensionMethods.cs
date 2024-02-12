using UnityEngine;

static class ExtensionMethods
{
    /// <summary>
    /// Rounds Vector3.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="decimalPlaces"></param>
    /// <returns></returns>
    public static Vector3 round(this Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }

    /// <summary>
    /// Rounds up Vector3.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="decimalPlaces"></param>
    /// <returns></returns>
    public static Vector3 ceil(this Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = Mathf.Pow(10,decimalPlaces);
        return round(vector3 + new Vector3(0.5f*multiplier, 0.5f*multiplier, 0.5f*multiplier), decimalPlaces);
    }

    /// <summary>
    /// Rounds down Vector3.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="decimalPlaces"></param>
    /// <returns></returns>
    public static Vector3 floor(this Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = Mathf.Pow(10, decimalPlaces);
        return round(vector3 - new Vector3(0.5f * multiplier, 0.5f * multiplier, 0.5f * multiplier), decimalPlaces);
    }
}
