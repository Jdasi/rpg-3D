using System;
using System.Collections.Generic;

public sealed partial class CharacterData
{
    public CharacterId CharacterId { get; }
    public TeamId TeamId { get; private set; }
    public CharacterTraits Traits { get; set; }
    public ModelConfig Model { get; private set; }
    public SkillManager Skills { get; }
    public StatBlock Stats { get; }
    public InventoryManager Inventory { get; }
    public ResistanceManager Resistances { get; }

    private readonly ConditionManager _conditionManager;
    private Dictionary<TalentIds, int> _talents;

    public CharacterData(CharacterId characterId)
    {
        CharacterId = characterId;
        Skills = new SkillManager();
        Stats = new StatBlock(characterId);
        Inventory = new InventoryManager(this);
        Resistances = new ResistanceManager(this);
        _conditionManager = new ConditionManager(this);
    }

    public void Init(CharacterConfig config)
    {
        TeamId = config.TeamId;
        Model = config.Model;
        
        for (int i = 0; i < config.Stats?.Length; ++i)
        {
            Stats.Set(config.Stats[i].Type, config.Stats[i].Value);
        }
        Stats.SetHealth(Stats.MaxHealth);

        if (config.Weapon != WeaponIds.Invalid)
        {
            Inventory.EquipWeapon(config.Weapon);
        }

        _talents = config.Talents != null ? new Dictionary<TalentIds, int>(config.Talents.Length) : null;
        for (int i = 0; i < config.Talents?.Length; ++i)
        {
            ref TalentData talent = ref config.Talents[i];
            Talents.SetRank(talent.Id, talent.Rank, this);
            _talents.Add(talent.Id, talent.Rank);
        }
    }

    public bool HasTalent(TalentIds talentId, out int rank)
    {
        if (_talents != null)
        {
            return _talents.TryGetValue(talentId, out rank);
        }

        rank = 0;
        return false;
    }

    public bool HasCondition(ConditionIds id)
    {
        return _conditionManager.Has(id);
    }

    public void SetCondition(ConditionIds id, bool set)
    {
        if (set)
        {
            _conditionManager.Apply(id);
        }
        else
        {
            _conditionManager.Remove(id);
        }
    }

    public void HandleCombatStart()
    {
        Inventory.HandleCombatStart();
    }

    public void HandleCombatEnd()
    {
        Inventory.HandleCombatEnd();
    }

    public void ProcessSkillCostConfig(SkillIds skillId, SkillCostConfig config, bool spend)
    {
        if (config == null)
        {
            return;
        }

        Inventory.ProcessSkillCostConfig(skillId, config);
        _conditionManager.ProcessSkillCostConfig(skillId, config);

        if (spend)
        {
            // TODO ..
        }
    }

    public void ProcessTargetingConfig(SkillIds skillId, ITargetingConfig config)
    {
        Inventory.ProcessTargetingConfig(skillId, config);
        _conditionManager.ProcessTargetingConfig(skillId, config);
    }

    public void ProcessIncomingEffect(EffectResolveContext context, EffectResolveContext.ProcessStages stage)
    {
        Inventory.ProcessIncomingEffect(context, stage);
        _conditionManager.ProcessIncomingEffect(context, stage);
    }

    public void ProcessOutgoingEffect(EffectResolveContext context, EffectResolveContext.ProcessStages stage)
    {
        Inventory.ProcessOutgoingEffect(context, stage);
        _conditionManager.ProcessOutgoingEffect(context, stage);
        BoostOutgoingElements();

        return;

        void BoostOutgoingElements()
        {
            if (stage != EffectResolveContext.ProcessStages.Finalised)
            {
                return;
            }

            for (int i = 0; i < context.Effects.Count; ++i)
            {
                if (context.Effects[i] is not EffectTypes.DealElement element)
                {
                    continue;
                }

                StatTypes boostStat = element.Type.ToElementBoostStat();
                int boost = Stats.Get(boostStat);

                if (boost <= 0)
                {
                    continue;
                }

                float boostFactor = boost / 100f;
                element.FinalValue += (int)Math.Ceiling(element.FinalValue * boostFactor);
            }
        }
    }

    public void ResolveIncomingEffect(EffectResolveContext context)
    {
        Resistances.ResolveIncomingEffect(context);
        _conditionManager.Purge();

        EffectResolveContext.FinalValues finalValues = context.GetFinalValues();

        if (finalValues.Heal > 0)
        {
            Stats.AdjustHealth(finalValues.Heal);
            LocalEvents.CharacterHealthEffect.Invoke(new LocalEventData.CharacterHealthEffect
            {
                CharacterId = CharacterId,
                Amount = finalValues.Heal,
                Flags = context.TempFlags,
                Source = context.Source,
            });
        }

        if (finalValues.Damage > 0 || finalValues.Absorb > 0)
        {
            Stats.AdjustHealth(-finalValues.Damage);
            LocalEvents.CharacterHealthEffect.Invoke(new LocalEventData.CharacterHealthEffect
            {
                CharacterId = CharacterId,
                IsDamage = true,
                Amount = finalValues.Damage,
                AbsorbAmount = finalValues.Absorb,
                Flags = context.TempFlags,
                Source = context.Source,
            });
        }

        for (int i = 0; i < context.Effects?.Count; ++i)
        {
            switch (context.Effects[i])
            {
                case EffectTypes.ApplyCondition e:
                {
                    _conditionManager.Apply(e.Id, e.Duration, e.Stack, context.Source);
                } break;

                case EffectTypes.RemoveCondition e:
                {
                    _conditionManager.Remove(e.Id, e.Stack, context.Source);
                } break;

                case EffectTypes.AddForce e:
                {
                    BoardControl.Instance.TryGetToken(CharacterId, out Token token);
                    token.AddForce(e.Direction * e.Amount);
                } break;
            }
        }
    }
}
