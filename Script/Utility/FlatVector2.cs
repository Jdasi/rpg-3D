using System;
using UnityEngine;

[Serializable]
public struct FlatVector2
{
    public float x;
    public float y;

    public FlatVector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    // Implicit conversion to Vector3 (on XZ plane)
    public static implicit operator Vector3(FlatVector2 fv)
    {
        return new Vector3(fv.x, 0f, fv.y);
    }

    // Implicit conversion from Vector3 (using X and Z)
    public static implicit operator FlatVector2(Vector3 v)
    {
        return new FlatVector2(v.x, v.z);
    }

    public override string ToString()
    {
        return $"FlatVector2({x}, {y})";
    }
}