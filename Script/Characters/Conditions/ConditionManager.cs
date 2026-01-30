using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConditionManager : IConditionManager
{
    public const int PERMANENT_DURATION = -1;
    public const int ALL_STACKS = -1;

    public ConditionIds[] AllExisting => _conditions?.Select(elem => elem.Id).ToArray();
    public ConditionIds[] AllHarmful => _conditions?.Where(elem => elem.IsHarmful).Select(elem => elem.Id).ToArray();
    public ConditionIds[] AllBeneficial => _conditions?.Where(elem => !elem.IsHarmful).Select(elem => elem.Id).ToArray();

    public event IConditionManager.ProcessSkillConfigHandler OnProcessSkillCostConfig;
    public event IConditionManager.ProcessTargetingConfigHandler OnProcessTargetingConfig;
    public event IConditionManager.ProcessIncomingEffectHandler OnProcessIncomingEffect;
    public event IConditionManager.ProcessOutgoingEffectHandler OnProcessOutgoingEffect;

    public CharacterData Owner { get; }

    private List<Condition> _conditions;

    public ConditionManager(CharacterData owner)
    {
        Owner = owner;
    }

    public bool Has(ConditionIds id)
    {
        return TryGet(id, out _);
    }

    public bool Apply(ConditionIds id, int duration = PERMANENT_DURATION, int stack = 1, EffectSourceInfo source = null)
    {
        Debug.Assert(id != ConditionIds.Invalid, $"[ConditionManager] Apply - invalid id: {id}");
        Debug.Assert(stack > 0, $"[ConditionManager] Apply - invalid stack: {stack}");

        Condition condition;

        if (TryGet(id, out var index))
        {
            condition = _conditions[index];
            int prevStack = condition.Stack;

            switch (condition.StackType)
            {
                case ConditionStackTypes.Overwrite: condition.Stack = stack; break;
                case ConditionStackTypes.Additive: condition.Stack += stack; break;
            }

            condition.Renew(Owner, duration, prevStack, condition.Stack, source);

            return true;
        }

        condition = ConditionFactory.Create(id, duration);
        condition.Start(duration, stack, this, source);

        _conditions ??= new List<Condition>();
        _conditions.Add(condition);

        LocalEvents.CharacterConditionAdded.Invoke(new LocalEventData.CharacterConditionAdded
        {
            CharacterId = Owner.CharacterId,
            ConditionId = id,
            Source = source,
        });

        return true;
    }

    public bool Remove(ConditionIds id, int stack = ALL_STACKS, EffectSourceInfo source = null)
    {
        if (id == ConditionIds.Invalid)
        {
            return false;
        }

        if (!TryGet(id, out var index))
        {
            return false;
        }

        Condition condition = _conditions[index];

        if (stack == ALL_STACKS || stack >= condition.Stack)
        {
            Remove(index, source);
        }
        else
        {
            _conditions[index].Stack -= stack;
        }

        return true;
    }

    public void Clear()
    {
        if (_conditions == null)
        {
            return;
        }

        for (int i = _conditions.Count - 1; i >= 0; --i)
        {
            Remove(i);
        }
    }

    public void Tick(TickTiming timing)
    {
        if (_conditions == null)
        {
            return;
        }

        for (int i = _conditions.Count - 1; i >= 0; --i)
        {
            Condition condition = _conditions[i];
            condition.Tick(Owner, timing);

            if (condition.HasExpired)
            {
                Remove(i);
            }
        }
    }

    public void ProcessSkillCostConfig(SkillIds skillId, SkillCostConfig config)
    {
        OnProcessSkillCostConfig?.Invoke(skillId, config);
    }

    public void ProcessTargetingConfig(SkillIds skillId, ITargetingConfig config)
    {
        OnProcessTargetingConfig?.Invoke(skillId, config);
    }

    public void ProcessIncomingEffect(EffectResolveContext context, EffectResolveContext.ProcessStages stage)
    {
        OnProcessIncomingEffect?.Invoke(context, stage);
    }

    public void ProcessOutgoingEffect(EffectResolveContext context, EffectResolveContext.ProcessStages stage)
    {
        OnProcessOutgoingEffect?.Invoke(context, stage);
    }

    public void Purge()
    {
        if (_conditions == null)
        {
            return;
        }

        for (int i = _conditions.Count - 1; i >= 0; --i)
        {
            if (_conditions[i].HasExpired)
            {
                Remove(i);
            }
        }
    }

    private bool TryGet(ConditionIds id, out int index)
    {
        for (int i = 0; i < _conditions?.Count; ++i)
        {
            if (_conditions[i].Id == id)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    private void Remove(int index, EffectSourceInfo source = null)
    {
        Debug.Assert(index >= 0 && index < _conditions?.Count, $"[ConditionManager] Remove - invalid index: {index}");
        _conditions[index].End(this);

        LocalEvents.CharacterConditionRemoved.Invoke(new LocalEventData.CharacterConditionRemoved
        {
            CharacterId = Owner.CharacterId,
            ConditionId = _conditions[index].Id,
            Source = source,
        });

        if (_conditions.Count == 1)
        {
            _conditions = null;
            return;
        }

        _conditions.RemoveAt(index);
    }
}
