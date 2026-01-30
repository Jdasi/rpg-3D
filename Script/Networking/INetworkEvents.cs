
public interface INetworkEvents
{
    // full event (where NetPeer is needed)
    public void Subscribe<T>(NetworkMessageHandler.MessagePeerHandler<T> onReceive) where T : INetworkMessage, new();
    public void Unsubscribe<T>(NetworkMessageHandler.MessagePeerHandler<T> onReceive) where T : INetworkMessage, new();

    // partial event (where NetPeer isn't needed)
    public void Subscribe<T>(NetworkMessageHandler.MessageHandler<T> onReceive) where T : INetworkMessage, new();
    public void Unsubscribe<T>(NetworkMessageHandler.MessageHandler<T> onReceive) where T : INetworkMessage, new();
}
