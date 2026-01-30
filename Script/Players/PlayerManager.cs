using LiteNetLib;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public const string TEMP_USERNAME = "Client";

    public static PlayerId LocalId { get; private set; } = PlayerId.Invalid;
    public static IReadOnlyList<PlayerInfo> Players => _instance?._players;

    private static PlayerManager _instance;

    private List<PlayerInfo> _players;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        _players = new List<PlayerInfo>();

        LocalEvents.ServerStarted.Subscribe(OnServerStarted);
        LocalEvents.Disconnected.Subscribe(OnDisconnected);

        NetworkManager.Subscribe<NetworkMessages.ServerWelcome>(OnServerWelcome);
        NetworkManager.Subscribe<NetworkMessages.PlayerStateFlag>(OnPlayerStateFlag);
        NetworkManager.Subscribe<NetworkMessages.ClientJoined>(OnClientJoined);
        NetworkManager.Subscribe<NetworkMessages.ClientDisconnected>(OnClientDisconnected);
        NetworkManager.Subscribe<NetworkMessages.ExistingClient>(OnExistingClient);
    }

    private void OnDestroy()
    {
        if (_instance != this)
        {
            return;
        }

        LocalEvents.ServerStarted.Unsubscribe(OnServerStarted);
        LocalEvents.Disconnected.Unsubscribe(OnDisconnected);

        NetworkManager.Unsubscribe<NetworkMessages.ServerWelcome>(OnServerWelcome);
        NetworkManager.Unsubscribe<NetworkMessages.PlayerStateFlag>(OnPlayerStateFlag);
        NetworkManager.Unsubscribe<NetworkMessages.ClientJoined>(OnClientJoined);
        NetworkManager.Unsubscribe<NetworkMessages.ClientDisconnected>(OnClientDisconnected);
        NetworkManager.Unsubscribe<NetworkMessages.ExistingClient>(OnExistingClient);
    }

    public static void SetLocalStateFlag(PlayerStateFlags flag, bool value)
    {
        if (!_instance.TryGetPlayer(LocalId, out PlayerInfo localPlayer))
        {
            return;
        }

        if (!localPlayer.TrySetStateFlag(flag, value))
        {
            return;
        }

        if (!NetworkManager.IsHost)
        {
            NetworkManager.SendMessage(new NetworkMessages.PlayerStateFlag
            {
                PlayerId = LocalId,
                Flag = flag,
                Value = value,
            }, DeliveryMethod.ReliableOrdered);
        }
    }

    private void AddOrRenamePlayer(PlayerId id, string username)
    {
        int index = GetPlayerIndex(id);

        if (index >= 0)
        {
            _players[index].Username = username;
        }
        else
        {
            _players.Add(new PlayerInfo
            {
                PlayerId = id,
                Username = username,
            });

            _players.Sort((a, b) => a.PlayerId.CompareTo(b.PlayerId));
            LocalEvents.PlayerAdded.Invoke(id);
        }
    }

    private void RemovePlayer(PlayerId id)
    {
        int index = GetPlayerIndex(id);

        if (index >= 0)
        {
            _players.RemoveAt(index);
            LocalEvents.PlayerRemoved.Invoke(id);
        }
    }

    private int GetPlayerIndex(PlayerId id)
    {
        for (int i = 0; i < _players.Count; ++i)
        {
            if (_players[i].PlayerId == id)
            {
                return i;
            }
        }

        return -1;
    }

    private bool TryGetPlayer(PlayerId id, out PlayerInfo player)
    {
        for (int i = 0; i < _players.Count; ++i)
        {
            if (_players[i].PlayerId == id)
            {
                player = _players[i];
                return true;
            }
        }

        player = null;
        return false;
    }

    private void OnServerStarted()
    {
        LocalId = PlayerId.One;
        AddOrRenamePlayer(LocalId, "Server");
    }

    private void OnDisconnected()
    {
        _players.Clear();
        LocalId = PlayerId.Invalid;
    }

    private void OnServerWelcome(NetworkMessages.ServerWelcome data)
    {
        LocalId = data.PlayerId;
        AddOrRenamePlayer(data.PlayerId, TEMP_USERNAME);
    }

    private void OnClientJoined(NetworkMessages.ClientJoined data)
    {
        AddOrRenamePlayer(data.PlayerId, data.Username);
    }

    private void OnPlayerStateFlag(NetworkMessages.PlayerStateFlag msg, NetPeer peer)
    {
        if (!TryGetPlayer(msg.PlayerId, out PlayerInfo player))
        {
            return;
        }

        player.TrySetStateFlag(msg.Flag, msg.Value);
    }

    private void OnClientDisconnected(NetworkMessages.ClientDisconnected data)
    {
        RemovePlayer(data.PlayerId);
    }

    private void OnExistingClient(NetworkMessages.ExistingClient data)
    {
        AddOrRenamePlayer(data.PlayerId, data.Username);
    }
}
