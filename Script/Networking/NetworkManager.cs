using LiteNetLib;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public const int MAX_PLAYERS = 5;

    public static bool IsHost => IsConnected && (_instance?._netPlayer?.IsHost ?? false);
    public static bool IsConnecting => !IsConnected && _instance?._netPlayer != null;
    public static bool IsConnected => _instance?._netPlayer?.IsConnected ?? false;
    public static int Ping => _instance?._netPlayer?.Ping ?? 0;
    public static EntityId NextEntityId => _instance._nextEntityId++;

    public const string CONNECTION_KEY = "j2d2s0t3t2v5g";
    public const string TEST_SERVER_ADDRESS = "localhost";
    public const int TEST_SERVER_PORT = 61953;

    private static NetworkManager _instance;

    private NetworkMessageHandler _messageHandler;
    private NetworkPlayer _netPlayer;
    private EntityId _nextEntityId;

    public static void Subscribe<T>(NetworkMessageHandler.MessagePeerHandler<T> onReceive) where T : INetworkMessage, new()
    {
        _instance?._messageHandler?.Subscribe(onReceive);
    }

    public static void Unsubscribe<T>(NetworkMessageHandler.MessagePeerHandler<T> onReceive) where T : INetworkMessage, new()
    {
        _instance?._messageHandler?.Unsubscribe(onReceive);
    }

    public static void Subscribe<T>(NetworkMessageHandler.MessageHandler<T> onReceive) where T : INetworkMessage, new()
    {
        _instance?._messageHandler?.Subscribe(onReceive);
    }

    public static void Unsubscribe<T>(NetworkMessageHandler.MessageHandler<T> onReceive) where T : INetworkMessage, new()
    {
        _instance?._messageHandler?.Unsubscribe(onReceive);
    }

    public static void StartLocal()
    {
        _instance.InitNetworkPlayer(new NetworkLocal());
    }

    public static void StartHost()
    {
        _instance.InitNetworkPlayer(new NetworkHost());
    }

    public static void JoinHost()
    {
        _instance.InitNetworkPlayer(new NetworkClient());
    }

    public static void Disconnect()
    {
        _instance.DisconnectInternal();
    }

    public static void SendMessage(INetworkMessage msg, DeliveryMethod deliveryMethod, PlayerId exceptPlayer = PlayerId.Invalid)
    {
        _instance.SendInternal(msg, deliveryMethod, exceptPlayer);
    }

    public static void SendMessageDirect(INetworkMessage msg, DeliveryMethod deliveryMethod, PlayerId toPlayer)
    {
        _instance.SendDirectInternal(msg, deliveryMethod, toPlayer);
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }

        _instance = this;
        _messageHandler = new NetworkMessageHandler();
    }

    private void OnDestroy()
    {
        if (_instance != this)
        {
            return;
        }

        _instance = null;
        DisconnectInternal();
    }

    private void Update()
    {
        _netPlayer?.Update();
    }

    private void InitNetworkPlayer(NetworkPlayer player)
    {
        if (_netPlayer != null)
        {
            Debug.Log("[NetworkManager] InitNetworkPlayer - already set");
            return;
        }

        _netPlayer = player;
        _netPlayer.ConnectedStatusChanged += OnConnectedStatusChanged;
        _netPlayer.Init(TEST_SERVER_ADDRESS, TEST_SERVER_PORT, _messageHandler);
    }

    private void DisconnectInternal()
    {
        if (_netPlayer == null)
        {
            return;
        }

        _netPlayer.Cleanup();
        _netPlayer = null;

        LocalEvents.Disconnected.Invoke();
    }

    private void SendInternal(INetworkMessage msg, DeliveryMethod deliveryMethod, PlayerId exceptPlayer)
    {
        if (_netPlayer == null)
        {
            return;
        }

        _netPlayer.SendRequest(msg, deliveryMethod, exceptPlayer);
    }

    private void SendDirectInternal(INetworkMessage msg, DeliveryMethod deliveryMethod, PlayerId toPlayer)
    {
        if (_netPlayer == null)
        {
            return;
        }

        _netPlayer.SendDirectRequest(msg, deliveryMethod, toPlayer);
    }

    private void OnConnectedStatusChanged(bool status)
    {
        if (!status)
        {
            DisconnectInternal();
        }

        Debug.Log($"[NetworkManager] OnConnectedStatusChanged - status: {status}");
    }
}
