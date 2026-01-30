using UnityEngine;

public abstract class Condition
{
    public ConditionIds Id { get; private set; }
    public TickTiming TickTiming { protected get; init; }
    public ConditionStackTypes StackType { get; init; }
    public int MaxStack { get; init; } = 1;
    public bool IsHarmful { get; init; }

    public bool HasExpired { get; protected set; }
    public int Duration { get; private set; }
    public int Stack { get => _stack; set => _stack = Mathf.Clamp(_stack + value, 1, MaxStack); }

    private int _stack;

    public void Init(ConditionIds id)
    {
        Id = id;
    }

    public void Start(int duration, int stack, IConditionManager manager, EffectSourceInfo source)
    {
        Stack = stack;
        OnStart(manager, source);
        RefreshDuration(duration);
    }

    public void End(IConditionManager manager)
    {
        OnEnd(manager);
    }

    public void Renew(CharacterData owner, int duration, int prevStack, int newStack, EffectSourceInfo source)
    {
        RefreshDuration(duration);
        OnRenew(owner, prevStack, newStack, source);
    }

    public void Tick(CharacterData owner, TickTiming timing)
    {
        if (TickTiming != timing)
        {
            return;
        }

        HasExpired |= --Duration <= 0;
        OnTick(owner);
    }

    protected virtual void OnStart(IConditionManager manager, EffectSourceInfo source) { }
    protected virtual void OnEnd(IConditionManager manager) { }
    protected virtual void OnRenew(CharacterData owner, int prevStack, int newStack, EffectSourceInfo source) { }
    protected virtual void OnTick(CharacterData owner) { }

    private void RefreshDuration(int duration)
    {
        Duration = Mathf.Max(duration, Duration);
    }
}
