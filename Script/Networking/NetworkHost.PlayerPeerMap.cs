using LiteNetLib;
using System.Collections.Generic;
using UnityEngine;

public partial class NetworkHost
{
    private class PlayerPeerMap
    {
        private readonly struct Mapping
        {
            public readonly PlayerId PlayerId;
            public readonly NetPeer NetPeer;

            public Mapping(PlayerId id, NetPeer peer)
            {
                PlayerId = id;
                NetPeer = peer;
            }
        }

        public int Count => _players.Count;

        private readonly List<Mapping> _players;

        private PlayerId _nextPlayerId = PlayerId.One;

        public PlayerPeerMap()
        {
            _players = new List<Mapping>(NetworkManager.MAX_PLAYERS);
        }

        public PlayerId Add(NetPeer peer)
        {
            if (TryGet(peer, out PlayerId id))
            {
                Debug.LogError($"[PlayerPeerMap] Add - peer [{peer}] already mapped to id: {id}");
                return PlayerId.Invalid;
            }

            PlayerId nextId = _nextPlayerId++;
            Debug.Log($"[PlayerPeerMap] Add - assigning {nextId} to peer [{peer}]");
            _players.Add(new Mapping(nextId, peer));

            return nextId;
        }

        public bool Remove(NetPeer peer)
        {
            int index = GetPeerIndex(peer);

            if (index >= 0)
            {
                _players.RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool Contains(NetPeer peer)
        {
            int index = GetPeerIndex(peer);
            return index >= 0;
        }

        public bool TryGet(NetPeer peer, out PlayerId id)
        {
            int index = GetPeerIndex(peer);
            id = index >= 0 ? _players[index].PlayerId : PlayerId.Invalid;
            return id != PlayerId.Invalid;
        }

        public bool TryGet(PlayerId id, out NetPeer peer)
        {
            for (int i = 0; i < _players.Count; ++i)
            {
                if (_players[i].PlayerId == id)
                {
                    peer = _players[i].NetPeer;
                    return true;
                }
            }

            peer = null;
            return false;
        }

        private int GetPeerIndex(NetPeer peer)
        {
            for (int i = 0; i < _players.Count; ++i)
            {
                if (_players[i].NetPeer == peer)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
