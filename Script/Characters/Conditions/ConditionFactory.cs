using UnityEngine;

public static class ConditionFactory
{
    public static Condition Create(ConditionIds id, int duration)
    {
        Condition condition = GetInternal(id);
        Debug.Assert(condition != null, $"[ConditionFactory] Create - failed for id: {id}");
        condition.Init(id);

        return condition;

        static Condition GetInternal(ConditionIds id)
        {
            return id switch
            {
                // ==============================================================================================================================================

                ConditionIds.Bleeding => new Condition_DamageTick
                {
                    IsHarmful = true,
                },

                ConditionIds.Burning => new Condition_DamageTick
                {
                    IsHarmful = true,
                },

                ConditionIds.SacredShield => new Condition_SacredShield { },

                ConditionIds.HolyPower => new Condition_ElementBoost
                {
                    MaxStack = 2,
                    Element = ElementTypes.Radiant,
                    CalculateBonus = stack => stack * 15,
                },

                // ==============================================================================================================================================
                _ => null,
            };
        }
    }
}
