using LiteNetLib;
using LiteNetLib.Utils;

public interface INetworkMessage
{
    NetworkMessageIds _Id { get; }
    bool LocalEcho => false; // as sender, automatically receive a local copy of this message
    bool AutoForward => false; // as host, automatically forward this message to others

    void Write(NetDataWriter writer);
    void Read(NetDataReader reader);
    bool ValidateLocal(NetworkPlayer player) => true;
    bool ValidateHost(NetworkHost host, NetPeer peer) => true;
}

public enum NetworkMessageIds : byte
{
    Invalid = 0,

    ClientIntroduction,
    ServerWelcome,
    ExistingClient,

    ClientJoined,
    ClientDisconnected,

    PlayerStateFlag,
    DoWalk,
    DoSkill,
    EntityDestroyed,
    SyncToken,
}

public static class NetworkMessages
{
    public class ClientIntroduction : INetworkMessage
    {
        public NetworkMessageIds _Id => NetworkMessageIds.ClientIntroduction;

        public PlayerId PlayerId;
        public string Username;

        public void Write(NetDataWriter writer)
        {
            writer.Put(PlayerId);
            writer.Put(Username);
        }

        public void Read(NetDataReader reader)
        {
            PlayerId = reader.GetPlayerId();
            Username = reader.GetString();
        }
    }

    public class ServerWelcome : INetworkMessage
    {
        public NetworkMessageIds _Id => NetworkMessageIds.ServerWelcome;

        public PlayerId PlayerId;

        public void Write(NetDataWriter writer)
        {
            writer.Put(PlayerId);
        }

        public void Read(NetDataReader reader)
        {
            PlayerId = reader.GetPlayerId();
        }
    }

    public class ClientJoined : INetworkMessage
    {
        public NetworkMessageIds _Id => NetworkMessageIds.ClientJoined;
        public bool LocalEcho => true;

        public PlayerId PlayerId;
        public string Username;

        public void Write(NetDataWriter writer)
        {
            writer.Put(PlayerId);
            writer.Put(Username);
        }

        public void Read(NetDataReader reader)
        {
            PlayerId = reader.GetPlayerId();
            Username = reader.GetString();
        }
    }

    public class ClientDisconnected : INetworkMessage
    {
        public NetworkMessageIds _Id => NetworkMessageIds.ClientDisconnected;
        public bool LocalEcho => true;

        public PlayerId PlayerId;

        public void Write(NetDataWriter writer)
        {
            writer.Put(PlayerId);
        }

        public void Read(NetDataReader reader)
        {
            PlayerId = reader.GetPlayerId();
        }
    }

    public class ExistingClient : INetworkMessage
    {
        public NetworkMessageIds _Id => NetworkMessageIds.ExistingClient;

        public PlayerId PlayerId;
        public string Username;

        public void Write(NetDataWriter writer)
        {
            writer.Put(PlayerId);
            writer.Put(Username);
        }

        public void Read(NetDataReader reader)
        {
            PlayerId = reader.GetPlayerId();
            Username = reader.GetString();
        }
    }

    public class EntityDestroyed : INetworkMessage
    {
        public NetworkMessageIds _Id => NetworkMessageIds.EntityDestroyed;

        public EntityId EntityId;

        public void Write(NetDataWriter writer)
        {
            writer.Put(EntityId);
        }

        public void Read(NetDataReader reader)
        {
            EntityId = reader.GetEntityId();
        }
    }

    public class DoWalk : INetworkMessage
    {
        public NetworkMessageIds _Id => NetworkMessageIds.DoWalk;
        public bool AutoForward => true;

        public FlatVector2 TargetPosition;

        public void Write(NetDataWriter writer)
        {
            writer.Put(TargetPosition);
        }

        public void Read(NetDataReader reader)
        {
            TargetPosition = reader.GetFlatVector2();
        }
    }

    public class DoSkill : INetworkMessage
    {
        public NetworkMessageIds _Id => NetworkMessageIds.DoSkill;
        public bool AutoForward => true;

        public int Seed;
        public SkillIds SkillId;
        public CharacterId CharacterId;
        public FlatVector2 TargetPosition;

        public void Write(NetDataWriter writer)
        {
            writer.Put(Seed);
            writer.Put(SkillId);
            writer.Put(CharacterId);
            writer.Put(TargetPosition);
        }

        public void Read(NetDataReader reader)
        {
            Seed = reader.GetInt();
            SkillId = reader.GetSkillId();
            CharacterId = reader.GetCharacterId();
            TargetPosition = reader.GetFlatVector2();
        }
    }

    public class PlayerStateFlag : INetworkMessage
    {
        public NetworkMessageIds _Id => NetworkMessageIds.PlayerStateFlag;

        public PlayerId PlayerId;
        public PlayerStateFlags Flag;
        public bool Value;

        public void Write(NetDataWriter writer)
        {
            if (Value)
            {
                Flag |= PlayerStateFlags.RESERVED_NETWORK_TRUE;
            }

            writer.Put(PlayerId);
            writer.Put((int)Flag);
        }

        public void Read(NetDataReader reader)
        {
            PlayerId = reader.GetPlayerId();
            Flag = (PlayerStateFlags)reader.GetInt();

            if (Flag.HasFlag(PlayerStateFlags.RESERVED_NETWORK_TRUE))
            {
                Flag &= ~PlayerStateFlags.RESERVED_NETWORK_TRUE;
                Value = true;
            }
        }
    }

    public class SyncToken : INetworkMessage
    {
        public NetworkMessageIds _Id => NetworkMessageIds.SyncToken;

        public CharacterId CharacterId;
        public FlatVector2 Position;

        public void Write(NetDataWriter writer)
        {
            writer.Put(CharacterId);
            writer.Put(Position);
        }

        public void Read(NetDataReader reader)
        {
            CharacterId = reader.GetCharacterId();
            Position = reader.GetFlatVector2();
        }
    }
}
