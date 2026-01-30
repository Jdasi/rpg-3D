
public class Condition_SacredShield : Condition
{
    private const int SHIELD_HEALTH = 10;

    private BarrierHandle _barrierHandle;

    protected override void OnStart(IConditionManager manager, EffectSourceInfo source)
    {
        _barrierHandle = manager.Owner.Resistances.AddBarrier(99, SHIELD_HEALTH, ElementTypes.PRESET_ALL_DAMAGE);

        LocalEvents.BarrierBroken.Subscribe(OnBarrierBroken);
    }

    protected override void OnEnd(IConditionManager manager)
    {
        LocalEvents.BarrierBroken.Unsubscribe(OnBarrierBroken);

        manager.Owner.Resistances.RemoveBarrier(_barrierHandle);
    }

    protected override void OnRenew(CharacterData owner, int prevStack, int newStack, EffectSourceInfo source)
    {
        owner.Resistances.SetBarrierHealth(_barrierHandle, SHIELD_HEALTH);
    }

    private void OnBarrierBroken(LocalEventData.BarrierBroken data)
    {
        if (data.Handle != _barrierHandle)
        {
            return;
        }

        HasExpired = true;
    }
}
