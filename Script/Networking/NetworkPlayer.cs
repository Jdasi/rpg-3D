using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public abstract class NetworkPlayer
{
    public delegate void ConnectedStatusChangedHandler(bool status);
    public event ConnectedStatusChangedHandler ConnectedStatusChanged;

    public bool IsConnected { get; private set; }
    public virtual bool IsHost => false;
    public virtual int Ping => 0;

    protected NetManager NetManager { get; private set; }
    protected NetworkMessageHandler MessageHandler { get; private set; }

    private NetDataWriter _netWriter;

    public void Init(string address, int port, NetworkMessageHandler messageHandler)
    {
        MessageHandler = messageHandler;
        OnInit(address, port);
    }

    public void Cleanup()
    {
        NetManager?.Stop();
        NetManager = null;

        RefreshConnectedStatus(false);
        OnCleanup();
    }

    public void Update()
    {
        NetManager?.PollEvents();
    }

    public void SendRequest(INetworkMessage msg, DeliveryMethod deliveryMethod, PlayerId exceptPlayer)
    {
        OnSendRequest(msg, deliveryMethod, exceptPlayer);
    }

    public void SendDirectRequest(INetworkMessage msg, DeliveryMethod deliveryMethod, PlayerId toPlayer)
    {
        OnSendDirectRequest(msg, deliveryMethod, toPlayer);
    }

    public bool ValidateAsHost(INetworkMessage msg, NetPeer peer)
    {
        if (!msg.ValidateHost(this as NetworkHost, peer))
        {
            Debug.LogError($"[NetworkPlayer] ValidateAsHost - failed for msg: {msg._Id} [peer: {peer}]");
            return false;
        }

        return true;
    }

    protected abstract void OnInit(string address, int port);
    protected virtual void OnCleanup() { }
    protected abstract void OnSendRequest(INetworkMessage msg, DeliveryMethod deliveryMethod, PlayerId exceptPlayer);
    protected abstract void OnSendDirectRequest(INetworkMessage msg, DeliveryMethod deliveryMethod, PlayerId toPlayer);
    protected virtual void OnPostProcessMessage(INetworkMessage msg, DeliveryMethod deliveryMethod, NetPeer peer) { }

    protected void InitNetManager(string address, int port, out EventBasedNetListener eventListener)
    {
        Debug.Assert(NetManager == null, "[NetworkPlayer] InitNetManager - NetManager already initialized");

        eventListener = new EventBasedNetListener();
        eventListener.NetworkReceiveEvent += OnNetworkReceiveEvent;
        eventListener.NetworkErrorEvent += OnNetworkError;

        NetManager = new NetManager(eventListener) { AutoRecycle = true };
        _netWriter = new NetDataWriter();
    }

    protected void RefreshConnectedStatus(bool status)
    {
        if (IsConnected == status)
        {
            return;
        }

        IsConnected = status;
        ConnectedStatusChanged?.Invoke(status);
    }

    protected void SendMessage(INetworkMessage msg, DeliveryMethod deliveryMethod, NetPeer exceptPeer = null)
    {
        if (!TryPrepareMessage(msg))
        {
            return;
        }

        NetManager.SendToAll(_netWriter, deliveryMethod, exceptPeer);
        MessageHandler.SafeLocalEcho(msg);
    }

    protected void SendMessageDirect(INetworkMessage msg, DeliveryMethod deliveryMethod, NetPeer peer)
    {
        if (!TryPrepareMessage(msg))
        {
            return;
        }

        peer.Send(_netWriter, deliveryMethod);
        MessageHandler.SafeLocalEcho(msg);
    }

    private bool TryPrepareMessage(INetworkMessage msg)
    {
        if (!msg.ValidateLocal(this))
        {
            Debug.LogError($"[NetworkPlayer] TryPrepare - ValidateLocal failed for msg: {msg._Id}");
            return false;
        }

        _netWriter.Reset();
        _netWriter.Put(msg._Id);
        msg.Write(_netWriter);

        return true;
    }

    private void OnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        while (reader.AvailableBytes > 0)
        {
            NetworkMessageIds id = reader.GetMessageId();

            if (MessageHandler.TryHandle(this, id, reader, peer, out INetworkMessage msg))
            {
                OnPostProcessMessage(msg, deliveryMethod, peer);
            }
            else
            {
                Debug.LogError($"[NetworkPlayer] OnNetworkReceiveEvent - message handle failed: {id} (available bytes: {reader.AvailableBytes}");
            }
        }
    }

    private void OnNetworkError(System.Net.IPEndPoint endPoint, System.Net.Sockets.SocketError socketError)
    {
        Debug.Log($"[NetworkPlayer] OnNetworkError - {socketError}");
    }
}
