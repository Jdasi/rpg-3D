
public static class LocalEventData
{
    public class SkillStarted
    {
        public SkillIds SkillId;
        public CharacterId CharacterId;
    }

    public class SkillFinished
    {
        public SkillIds SkillId;
        public CharacterId CharacterId;
        public SkillEffectTracker Tracker;
        public GeneralTraits Traits;
    }

    public class BarrierBroken
    {
        public BarrierHandle Handle;
        public EffectSourceInfo Source;
    }

    public class CharacterHealthChanged
    {
        public CharacterId CharacterId;
        public int Health;
        public int MaxHealth;
    }

    public class CharacterHealthEffect
    {
        public CharacterId CharacterId;
        public bool IsDamage;
        public int Amount;
        public int AbsorbAmount;
        public Bitset<EffectTempFlags> Flags;
        public EffectSourceInfo Source;
    }

    public class CharacterConditionAdded
    {
        public CharacterId CharacterId;
        public ConditionIds ConditionId;
        public EffectSourceInfo Source;
    }

    public class CharacterConditionRemoved
    {
        public CharacterId CharacterId;
        public ConditionIds ConditionId;
        public EffectSourceInfo Source;
    }
}
