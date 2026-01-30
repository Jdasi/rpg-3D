using LiteNetLib.Utils;

public static class NetSerializableCore
{
    public static void Put(this NetDataWriter writer, NetworkMessageIds id)
    {
        writer.Put((byte)id);
    }

    public static NetworkMessageIds GetMessageId(this NetDataReader reader)
    {
        return (NetworkMessageIds)reader.GetByte();
    }

    public static void Put(this NetDataWriter writer, PlayerId id)
    {
        writer.Put((byte)id);
    }

    public static PlayerId GetPlayerId(this NetDataReader reader)
    {
        return (PlayerId)reader.GetByte();
    }

    public static void Put(this NetDataWriter writer, EntityId id)
    {
        writer.Put((int)id);
    }

    public static EntityId GetEntityId(this NetDataReader reader)
    {
        return (EntityId)reader.GetInt();
    }

    public static void Put(this NetDataWriter writer, CharacterId id)
    {
        writer.Put((short)id);
    }

    public static CharacterId GetCharacterId(this NetDataReader reader)
    {
        return (CharacterId)reader.GetShort();
    }

    public static void Put(this NetDataWriter writer, SkillIds id)
    {
        writer.Put((short)id);
    }

    public static SkillIds GetSkillId(this NetDataReader reader)
    {
        return (SkillIds)reader.GetShort();
    }

    public static void Put(this NetDataWriter writer, FlatVector2 fv)
    {
        writer.Put(fv.x);
        writer.Put(fv.y);
    }

    public static FlatVector2 GetFlatVector2(this NetDataReader reader)
    {
        return new FlatVector2(
            reader.GetFloat(),
            reader.GetFloat()
            );
    }
}
