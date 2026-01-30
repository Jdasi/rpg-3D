using LiteNetLib;
using UnityEngine;

public abstract class NetworkEntity : MonoBehaviour
{
	public NetworkIdentity identity { get; } = new NetworkIdentity();
	public bool IsOwner => identity.playerId == PlayerManager.LocalId;

	public void SetIdentity(PlayerId playerId, EntityId entityId)
	{
		if (identity.entityId == EntityId.Invalid
			&& !NetworkManager.IsHost)
		{
			NetworkManager.Subscribe<NetworkMessages.EntityDestroyed>(OnEntityDestroyed);
		}

		identity.Set(playerId, entityId);
	}

	protected virtual void OnDestroy()
	{
		if (NetworkManager.IsHost)
		{
			NetworkManager.SendMessage(new NetworkMessages.EntityDestroyed
			{
				EntityId = identity.entityId,
			}, DeliveryMethod.ReliableOrdered);
		}
		else
		{
			NetworkManager.Unsubscribe<NetworkMessages.EntityDestroyed>(OnEntityDestroyed);
		}

		LocalEvents.EntityDestroyed.Invoke(this);
	}

	private void OnEntityDestroyed(NetworkMessages.EntityDestroyed data)
	{
		if (data.EntityId != identity.entityId)
		{
			return;
		}

		Destroy(this.gameObject);
	}
}
