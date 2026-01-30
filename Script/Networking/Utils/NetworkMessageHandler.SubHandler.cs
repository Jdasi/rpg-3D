using LiteNetLib;
using LiteNetLib.Utils;

public partial class NetworkMessageHandler
{
    private interface INetworkMessageSubHandler
    {
        bool IsValid();
        public bool TryHandle(NetworkPlayer netPlayer, NetDataReader reader, NetPeer peer, out INetworkMessage msg);
        public void LocalEcho(INetworkMessage msg);
    }

    private class SubHandler<T> : INetworkMessageSubHandler where T : INetworkMessage, new()
    {
        private MessagePeerHandler<T> _fullCallbacks;
        private MessageHandler<T> _partialCallbacks;

        public bool IsValid()
        {
            return _fullCallbacks != null || _partialCallbacks != null;
        }

        public void Add(MessagePeerHandler<T> callback)
        {
            _fullCallbacks += callback;
        }

        public void Remove(MessagePeerHandler<T> callback)
        {
            _fullCallbacks -= callback;
        }

        public void Add(MessageHandler<T> callback)
        {
            _partialCallbacks += callback;
        }

        public void Remove(MessageHandler<T> callback)
        {
            _partialCallbacks -= callback;
        }

        public bool TryHandle(NetworkPlayer netPlayer, NetDataReader reader, NetPeer peer, out INetworkMessage outMsg)
        {
            var msg = new T();
            msg.Read(reader);

            if (netPlayer.IsHost
                && !netPlayer.ValidateAsHost(msg, peer))
            {
                outMsg = null;
                return false;
            }

            _fullCallbacks?.Invoke(msg, peer);
            _partialCallbacks?.Invoke(msg);

            outMsg = msg;
            return true;
        }

        public void LocalEcho(INetworkMessage msg)
        {
            _partialCallbacks?.Invoke((T)msg);
        }
    }
}
