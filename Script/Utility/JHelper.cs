using System.Text.RegularExpressions;
using UnityEngine;

// VS 2019 JANK
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}

public static class JHelper
{
    public static Vector3 SnapToTokenPlane(this Vector3 point)
    {
        point.y = 0;
        return point;
    }

    public static Vector3 SnapToEffectPlane(this Vector3 point)
    {
        const float Y_OFFSET = 0.2f;
        point.y = Y_OFFSET;
        return point;
    }

    public static float FlatDist(this Vector3 from, Vector3 to)
    {
        to.y = from.y;
        return (to - from).magnitude;
    }

    public static float FlatDistSqr(this Vector3 from, Vector3 to)
    {
        to.y = from.y;
        return (to - from).sqrMagnitude;
    }

    public static Vector3 FlatDir(this Vector3 from, Vector3 to)
    {
        to.y = from.y;
        return (to - from).normalized;
    }

    public static bool IsInArea(this Vector3 a, Vector3 b, float radius, bool ignoreY = true)
    {
        float radiusSqr = radius * radius;

        if (ignoreY)
        {
            return a.FlatDistSqr(b) <= radiusSqr;
        }
        else
        {
            return Vector3.SqrMagnitude(a - b) <= radiusSqr;
        }
    }

    /// <summary>
    /// Spaces out a "StringLikeThis" into "String Like This".
    /// </summary>
    public static string SpaceOut(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        return Regex.Replace(str, "(?<=[a-z])([A-Z])", " $1" );
    }
}
