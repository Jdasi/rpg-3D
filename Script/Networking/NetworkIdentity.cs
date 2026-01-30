
public class NetworkIdentity
{
	public PlayerId playerId { get; private set; } = PlayerId.Invalid;
	public EntityId entityId { get; private set; } = EntityId.Invalid;

	public delegate void IdentityChangedHandler(NetworkIdentity identity);
	public IdentityChangedHandler IdentityChanged;

	public void Set(PlayerId playerId, EntityId entityId)
	{
		this.playerId = playerId;
		this.entityId = entityId;

		IdentityChanged?.Invoke(this);
	}

	public void ChangeAuthority(PlayerId playerId)
	{
		this.playerId = playerId;

		IdentityChanged?.Invoke(this);
	}
}
