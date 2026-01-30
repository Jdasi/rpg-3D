using System;
using UnityEngine;

public interface IEffectComponent { }

public static class EffectTypes
{
    public class DealElement : IEffectComponent
    {
        public const float BASE_VARIANCE = 0.15f;

        public readonly ElementTypes Type;

        public int BaseValue { get => _baseValue; set => _baseValue = Math.Max(1, value); }
        public int FinalValue { get => _finalValue; set => _finalValue = Math.Max(1, value); }
        public int AbsorbValue;
        public bool IsDamage;

        private int _baseValue;
        private int _finalValue;

        public DealElement(ElementTypes id, int value, float variance = BASE_VARIANCE)
        {
            Type = id;
            BaseValue = value;
            FinalValue = variance > 0 ? Rand.Roll(value, variance) : value;
            IsDamage = Type != ElementTypes.Restoration;
        }
    }

    public class ApplyCondition : IEffectComponent
    {
        public readonly ConditionIds Id;
        public int Duration;
        public int Stack;

        public ApplyCondition(ConditionIds id, int duration, int stack = 1)
        {
            Id = id;
            Duration = duration;
            Stack = stack;
        }
    }

    public class RemoveCondition : IEffectComponent
    {
        public readonly ConditionIds Id;
        public int Stack;

        public RemoveCondition(ConditionIds id, int stack = -1)
        {
            Id = id;
            Stack = stack;
        }
    }

    public class HitChance : IEffectComponent
    {
        public int Amount;

        public HitChance(int amount)
        {
            Amount = amount;
        }
    }

    public class CritChance : IEffectComponent
    {
        public int Amount;

        public CritChance(int amount)
        {
            Amount = amount;
        }
    }

    public class AddForce : IEffectComponent
    {
        public Vector3 Direction;
        public float Amount;

        public AddForce(Vector3 direction, float amount)
        {
            Direction = direction;
            Amount = amount;
        }
    }
}
