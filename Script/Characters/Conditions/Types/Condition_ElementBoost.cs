
public class Condition_ElementBoost : Condition
{
    public ElementTypes Element { init => _boostStat = value.ToElementBoostStat(); }

    public delegate int BonusCalculator(int stack);
    public BonusCalculator CalculateBonus { get; init; }

    private StatTypes _boostStat;

    protected override void OnStart(IConditionManager manager, EffectSourceInfo source)
    {
        manager.Owner.Stats.AddModifier(_boostStat, Modify);
    }

    protected override void OnEnd(IConditionManager manager)
    {
        manager.Owner.Stats.RemoveModifier(_boostStat, Modify);
    }

    private int Modify(int value)
    {
        return value + CalculateBonus(Stack);
    }
}
