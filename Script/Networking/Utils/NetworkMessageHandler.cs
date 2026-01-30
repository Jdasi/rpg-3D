using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class NetworkMessageHandler : INetworkEvents
{
    public delegate void MessagePeerHandler<T>(T msg, NetPeer peer) where T : INetworkMessage;
    public delegate void MessageHandler<T>(T msg) where T : INetworkMessage;

    private readonly Dictionary<NetworkMessageIds, Type> _messageIdTypeLookup;
    private readonly Dictionary<Type, INetworkMessageSubHandler> _subHandlers;

    public NetworkMessageHandler()
    {
        _messageIdTypeLookup = new Dictionary<NetworkMessageIds, Type>();
        _subHandlers = new Dictionary<Type, INetworkMessageSubHandler>();

        // shared
        Register<NetworkMessages.ClientJoined>();
        Register<NetworkMessages.ClientDisconnected>();
        Register<NetworkMessages.DoWalk>();
        Register<NetworkMessages.DoSkill>();

        // server only?
        Register<NetworkMessages.ClientIntroduction>();
        Register<NetworkMessages.PlayerStateFlag>();

        // client only?
        Register<NetworkMessages.ServerWelcome>();
        Register<NetworkMessages.ExistingClient>();
        Register<NetworkMessages.EntityDestroyed>();
        Register<NetworkMessages.SyncToken>();

        return;

        void Register<T>() where T : INetworkMessage, new()
        {
            var template = new T();
            var type = typeof(T);

            _messageIdTypeLookup.Add(template._Id, type);
            _subHandlers.Add(type, new SubHandler<T>());
        }
    }

    public void Subscribe<T>(MessagePeerHandler<T> onReceive) where T : INetworkMessage, new()
    {
        ((SubHandler<T>)_subHandlers[typeof(T)]).Add(onReceive);
    }

    public void Unsubscribe<T>(MessagePeerHandler<T> onReceive) where T : INetworkMessage, new()
    {
        ((SubHandler<T>)_subHandlers[typeof(T)]).Remove(onReceive);
    }

    public void Subscribe<T>(MessageHandler<T> onReceive) where T : INetworkMessage, new()
    {
        ((SubHandler<T>)_subHandlers[typeof(T)]).Add(onReceive);
    }

    public void Unsubscribe<T>(MessageHandler<T> onReceive) where T : INetworkMessage, new()
    {
        ((SubHandler<T>)_subHandlers[typeof(T)]).Remove(onReceive);
    }

    public void SafeLocalEcho(INetworkMessage msg)
    {
        if (!msg.LocalEcho)
        {
            return;
        }

        var type = _messageIdTypeLookup[msg._Id];
        _subHandlers[type].LocalEcho(msg);
    }

    public bool TryHandle(NetworkPlayer netPlayer, NetworkMessageIds id, NetDataReader reader, NetPeer peer, out INetworkMessage msg)
    {
        if (!_messageIdTypeLookup.TryGetValue(id, out Type type))
        {
            Debug.LogError($"[NetworkEventManager] TryHandle - type lookup failed for id: {id}");
            msg = null;
            return false;
        }

        if (!_subHandlers[type].TryHandle(netPlayer, reader, peer, out msg))
        {
            return false;
        }

        return true;
    }
}
