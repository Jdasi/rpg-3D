using LiteNetLib.Utils;
using UnityEngine;

public static class NetSerializableUnity
{
    public static void Put(this NetDataWriter writer, Vector2 v)
    {
        writer.Put(v.x);
        writer.Put(v.y);
    }

    public static Vector2 GetVector2(this NetDataReader reader)
    {
        return new Vector2(
            reader.GetFloat(),
            reader.GetFloat()
            );
    }

    public static void Put(this NetDataWriter writer, Vector3 v)
    {
        writer.Put(v.x);
        writer.Put(v.y);
        writer.Put(v.z);
    }

    public static Vector3 GetVector3(this NetDataReader reader)
    {
        return new Vector3(
            reader.GetFloat(),
            reader.GetFloat(),
            reader.GetFloat()
            );
    }
}
