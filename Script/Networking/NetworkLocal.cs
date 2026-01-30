using LiteNetLib;
using UnityEngine;

/// <summary>
/// Fake client for local play.
/// </summary>
public partial class NetworkLocal : NetworkPlayer
{
    public override bool IsHost => true;

    protected override void OnInit(string address, int port)
    {
        RefreshConnectedStatus(true);
        LocalEvents.ServerStarted.Invoke();
    }

    protected override void OnSendRequest(INetworkMessage msg, DeliveryMethod deliveryMethod, PlayerId exceptPlayer)
    {
        SendMessage(msg);
    }

    protected override void OnSendDirectRequest(INetworkMessage msg, DeliveryMethod deliveryMethod, PlayerId toPlayer)
    {
        SendMessage(msg);
    }

    private void SendMessage(INetworkMessage msg)
    {
        if (!msg.ValidateLocal(this))
        {
            Debug.LogError($"[NetworkLocal] SendMessage - ValidateLocal failed for msg: {msg._Id}");
            return;
        }

        MessageHandler.SafeLocalEcho(msg);
    }
}
