using LiteNetLib;
using UnityEngine;

/// <summary>
/// Client that isn't the host.
/// </summary>
public class NetworkClient : NetworkPlayer
{
    public override int Ping => _server.Ping;

    private NetPeer _server;

    protected override void OnInit(string address, int port)
    {
        InitNetManager(address, port, out EventBasedNetListener eventListener);

        eventListener.PeerConnectedEvent += OnPeerConnectedEvent;
        eventListener.PeerDisconnectedEvent += OnPeerDisconnectedEvent;

        NetManager.Start();
        NetManager.Connect(address, port, NetworkManager.CONNECTION_KEY);
    }

    protected override void OnSendRequest(INetworkMessage msg, DeliveryMethod deliveryMethod, PlayerId exceptPlayer)
    {
        Debug.Assert(exceptPlayer == PlayerId.Invalid, $"[NetworkClient] OnSendRequest - expected invalid exceptPlayer, received: {exceptPlayer}");
        SendMessageDirect(msg, deliveryMethod, _server);
    }

    protected override void OnSendDirectRequest(INetworkMessage msg, DeliveryMethod deliveryMethod, PlayerId toPlayer)
    {
        Debug.LogError($"[NetworkClient] OnSendRequest - unhandled request");
    }

    // connected to the server
    private void OnPeerConnectedEvent(NetPeer peer)
    {
        _server = peer;
        RefreshConnectedStatus(true);
        SendMessageDirect(new NetworkMessages.ClientIntroduction
        {
            Username = PlayerManager.TEMP_USERNAME,
        }, DeliveryMethod.ReliableOrdered, _server);
    }

    // disconnected from the server
    private void OnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        _server = null;
        RefreshConnectedStatus(false);
    }
}
