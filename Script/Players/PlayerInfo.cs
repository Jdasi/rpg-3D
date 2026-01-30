
public class PlayerInfo
{
    public PlayerId PlayerId;
    public string Username;

    private PlayerStateFlags _stateFlags;

    public bool HasStateFlag(PlayerStateFlags flag)
    {
        return _stateFlags.HasFlag(flag);
    }

    public bool TrySetStateFlag(PlayerStateFlags flag, bool value)
    {
        if (HasStateFlag(flag) == value)
        {
            return false;
        }

        if (value)
        {
            _stateFlags |= flag;
        }
        else
        {
            _stateFlags &= ~flag;
        }

        return true;
    }
}
