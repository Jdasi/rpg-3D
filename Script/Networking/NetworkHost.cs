using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

/// <summary>
/// Client acting as the host.
/// </summary>
public partial class NetworkHost : NetworkPlayer
{
    public override bool IsHost => true;

    private NetDataReader _netReader;
    private PlayerPeerMap _playerPeerMap;

    protected override void OnInit(string address, int port)
    {
        InitNetManager(address, port, out EventBasedNetListener eventListener);

        eventListener.ConnectionRequestEvent += OnConnectionRequested;
        eventListener.PeerConnectedEvent += OnPeerConnected;
        eventListener.PeerDisconnectedEvent += OnPeerDisconnected;

        MessageHandler.Subscribe<NetworkMessages.ClientIntroduction>(OnClientIntroductionMsg);

        _netReader = new NetDataReader();
        _playerPeerMap = new PlayerPeerMap();

        NetManager.Start(NetworkManager.TEST_SERVER_PORT);
        RefreshConnectedStatus(true);

        PlayerId playerId = _playerPeerMap.Add(null);
        LocalEvents.ServerStarted.Invoke();
    }

    protected override void OnCleanup()
    {
        NetworkManager.Unsubscribe<NetworkMessages.ClientIntroduction>(OnClientIntroductionMsg);
    }

    protected override void OnSendRequest(INetworkMessage msg, DeliveryMethod deliveryMethod, PlayerId exceptPlayer)
    {
        if (exceptPlayer == PlayerId.Invalid)
        {
            SendMessage(msg, deliveryMethod);
        }
        else if (_playerPeerMap.TryGet(exceptPlayer, out var exceptPeer))
        {
            SendMessage(msg, deliveryMethod, exceptPeer);
        }
        else
        {
            Debug.LogError($"[NetworkHost] OnSendRequest - unhandled request");
        }
    }

    protected override void OnSendDirectRequest(INetworkMessage msg, DeliveryMethod deliveryMethod, PlayerId toPlayer)
    {
        if (toPlayer == PlayerId.Invalid)
        {
            SendMessage(msg, deliveryMethod);
        }
        else if (_playerPeerMap.TryGet(toPlayer, out var toPeer))
        {
            SendMessageDirect(msg, deliveryMethod, toPeer);
        }
        else
        {
            Debug.LogError($"[NetworkHost] OnSendDirectRequest - unhandled request");
        }
    }

    protected override void OnPostProcessMessage(INetworkMessage msg, DeliveryMethod deliveryMethod, NetPeer peer)
    {
        if (!msg.AutoForward)
        {
            return;
        }

        if (_playerPeerMap.Count <= 2)
        {
            // no other recipients
            return;
        }

        SendMessage(msg, deliveryMethod, peer);
    }

    private void OnConnectionRequested(ConnectionRequest request)
    {
        if (NetManager.ConnectedPeersCount < NetworkManager.MAX_PLAYERS)
        {
            request.AcceptIfKey(NetworkManager.CONNECTION_KEY);
        }
        else
        {
            request.Reject();
        }
    }
        
    // a client joins the server
    private void OnPeerConnected(NetPeer peer)
    {
        Debug.Log($"[NetworkHost] OnPeerConnected - [{peer}] (peer count: {NetManager.ConnectedPeersCount})");
        // wait for them to send back an introduction..
    }

    // a client leaves the server
    private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        _playerPeerMap.TryGet(peer, out PlayerId playerId);

        if (!_playerPeerMap.Remove(peer))
        {
            Debug.LogError($"[NetworkHost] OnPeerDisconnected - failed to remove [{peer}] (peer count: {NetManager.ConnectedPeersCount})");
            return;
        }

        Debug.Log($"[NetworkHost] OnPeerDisconnected - [{peer}] (peer count: {NetManager.ConnectedPeersCount})");
        SendMessage(new NetworkMessages.ClientDisconnected
        {
            PlayerId = playerId,
        }, DeliveryMethod.ReliableOrdered);
    }

    private void OnClientIntroductionMsg(NetworkMessages.ClientIntroduction msg, NetPeer peer)
    {
        if (_playerPeerMap.Contains(peer))
        {
            Debug.LogError($"[NetworkHost] OnClientIntroductionMsg - already contained {msg.Username} [{peer}] (peer count: {NetManager.ConnectedPeersCount})");
            return;
        }

        Debug.Log($"[NetworkHost] OnClientIntroductionMsg - {msg.Username} [{peer}] (peer count: {NetManager.ConnectedPeersCount})");
        PlayerId playerId = _playerPeerMap.Add(peer);

        // handshake with new client
        SendMessageDirect(new NetworkMessages.ServerWelcome
        {
            PlayerId = playerId,
        }, DeliveryMethod.ReliableOrdered, peer);

        // inform new client of existing players
        foreach (PlayerInfo player in PlayerManager.Players)
        {
            if (playerId == player.PlayerId)
            {
                continue;
            }

            SendMessageDirect(new NetworkMessages.ExistingClient
            {
                PlayerId = player.PlayerId,
                Username = player.Username,
            }, DeliveryMethod.ReliableOrdered, peer);
        }

        // notify existing players of new client
        SendMessage(new NetworkMessages.ClientJoined
        {
            PlayerId = playerId,
            Username = msg.Username,
        }, DeliveryMethod.ReliableOrdered, peer);
    }
}
