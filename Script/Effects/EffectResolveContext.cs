using UnityEngine;

public enum EffectTempFlags
{
    None = 0,

    Hit = Bits.Bit0,
    Crit = Bits.Bit1,
    Blocked = Bits.Bit2,
    Absorbed = Bits.Bit3,
}

public class EffectResolveContext
{
    public enum ProcessStages
    {
        /// <summary>
        /// Before determining the success of the effect.
        /// </summary>
        BeforeTest = Bits.Bit0,

        /// <summary>
        /// After determining the success of the effect.
        /// </summary>
        AfterTest = Bits.Bit1,

        /// <summary>
        /// After all effects have been compiled.
        /// </summary>
        Finalised = Bits.Bit2,

        /// <summary>
        /// After all effects have been resolved. User only.
        /// </summary>
        Resolved = Bits.Bit3,
    }

    public readonly struct FinalValues
    {
        public readonly int Damage;
        public readonly int Heal;
        public readonly int Absorb;

        public FinalValues(int damage, int heal, int absorb)
        {
            Damage = damage;
            Heal = heal;
            Absorb = absorb;
        }
    }

    public GeneralTraits Traits { get; set; }
    public CharacterData User { get; init; } // aka inflictor
    public CharacterData Target { get; init; } // aka victim
    public EffectSourceInfo Source { get; init; }
    public EffectCollection Effects { get; } = new EffectCollection();
    public Bitset<EffectTempFlags> TempFlags;

    public ProcessStages _stageFlags;

    private EffectResolveContext() { }

    public static EffectResolveContext Create(GeneralTraits traits, CharacterData user, CharacterData target,
        DefenseTypes defenseType = DefenseTypes.None, SkillIds skillId = SkillIds.Invalid)
    {
        var context = new EffectResolveContext
        {
            Traits = traits,
            User = user,
            Target = target,
            Source = new EffectSourceInfo
            {
                CharacterId = user.CharacterId,
                SkillId = skillId,
            },
        };

        context.DetermineSuccess(defenseType);
        return context;
    }

    public static EffectResolveContext Create(SkillContext skillContext, CharacterData target,
        DefenseTypes defenseType = DefenseTypes.None)
    {
        var context = new EffectResolveContext
        {
            Traits = skillContext.Traits,
            User = skillContext.User.Data,
            Target = target,
            Source = new EffectSourceInfo
            {
                CharacterId = skillContext.User.Data.CharacterId,
                SkillId = skillContext.SkillId,
            },
        };

        context.DetermineSuccess(defenseType);
        return context;
    }

    public static EffectResolveContext Combine(EffectResolveContext a, EffectResolveContext b)
    {
        Debug.Assert(a.User == b.User, $"[EffectResolveContext] Combine - failed on User");
        Debug.Assert(a.Target == b.Target, $"[EffectResolveContext] Combine - failed on Target");
        Debug.Assert(a._stageFlags == b._stageFlags, $"[EffectResolveContext] Combine - failed on Stage");

        EffectResolveContext c = new EffectResolveContext
        {
            User = a.User,
            Target = a.Target,
            Source = a.Source,
        };

        c.Traits = a.Traits | b.Traits;

        c.Effects.Add(a.Effects);
        c.Effects.Add(b.Effects);

        c.TempFlags.Set(EffectTempFlags.Hit, a.TempFlags.IsSet(EffectTempFlags.Hit) || b.TempFlags.IsSet(EffectTempFlags.Hit));
        c.TempFlags.Set(EffectTempFlags.Crit, a.TempFlags.IsSet(EffectTempFlags.Crit) || b.TempFlags.IsSet(EffectTempFlags.Crit));
        c.TempFlags.Set(EffectTempFlags.Blocked, a.TempFlags.IsSet(EffectTempFlags.Blocked) || b.TempFlags.IsSet(EffectTempFlags.Blocked));

        c._stageFlags = a._stageFlags;

        return c;
    }

    public void DetermineSuccess(DefenseTypes defenseType = DefenseTypes.None)
    {
        Debug.Assert(!_stageFlags.HasFlag(ProcessStages.BeforeTest), $"[EffectResolveContext] DetermineSuccess - already determined success");
        TriggerStage(ProcessStages.BeforeTest);

        int userHitChance = User.Stats.Get(StatTypes.HitChance);
        int userCritChance = User.Stats.Get(StatTypes.CritChance);

        if (Effects.TryGet(out EffectTypes.HitChance hitModifier))
        {
            userHitChance += hitModifier.Amount;
        }

        if (Effects.TryGet(out EffectTypes.CritChance critModifier))
        {
            userCritChance += critModifier.Amount;
        }

        const int NO_DEFEND_CHANCE = 0;
        int targetDefendChance = defenseType switch
        {
            DefenseTypes.ArmorClass => Target.Stats.Get(StatTypes.ArmorClass),
            DefenseTypes.Reflex => Target.Stats.Get(StatTypes.Reflex),
            DefenseTypes.Fortitude => Target.Stats.Get(StatTypes.Fortitude),
            DefenseTypes.Psyche => Target.Stats.Get(StatTypes.Psyche),

            _ => NO_DEFEND_CHANCE,
        };

        TempFlags.Set(EffectTempFlags.Hit, targetDefendChance == NO_DEFEND_CHANCE
            || Rand.RollUnder(userHitChance - targetDefendChance));

        if (TempFlags.IsSet(EffectTempFlags.Hit)
            && !Target.Resistances.HasBarrier())
        {
            if (Traits.HasFlag(GeneralTraits.Potent))
            {
                TempFlags.Set(EffectTempFlags.Crit, Rand.RollUnder(userCritChance));
            }

            if (Traits.HasFlag(GeneralTraits.Blockable))
            {
                TempFlags.Set(EffectTempFlags.Blocked, Rand.RollUnder(Target.Stats.Get(StatTypes.BlockChance)));
            }
        }

        TriggerStage(ProcessStages.AfterTest);
    }

    public void Finalise()
    {
        Debug.Assert(!_stageFlags.HasFlag(ProcessStages.Finalised), $"[EffectResolveContext] Finalise - already finalised");

        if (!_stageFlags.HasFlag(ProcessStages.BeforeTest))
        {
            DetermineSuccess();
        }

        bool hit = TempFlags.IsSet(EffectTempFlags.Hit);
        bool crit = TempFlags.IsSet(EffectTempFlags.Crit);
        bool blocked = TempFlags.IsSet(EffectTempFlags.Blocked);

        for (int i = 0; i < Effects?.Count; ++i)
        {
            switch (Effects[i])
            {
                case EffectTypes.DealElement e:
                {
                    if (hit)
                    {
                        if (crit)
                        {
                            e.FinalValue = e.FinalValue + (int)(e.BaseValue * 0.5f);
                        }

                        if (blocked)
                        {
                            e.FinalValue = (int)(e.FinalValue * 0.6f);
                        }
                    }
                    else
                    {
                        e.FinalValue = (int)(e.FinalValue * 0.4f);
                    }
                } break;
            }
        }

        TriggerStage(ProcessStages.Finalised);
    }

    public void Resolve()
    {
        if (!_stageFlags.HasFlag(ProcessStages.Finalised))
        {
            Finalise();
        }

        Target.ResolveIncomingEffect(this);
        TriggerStage(ProcessStages.Resolved);
    }

    public T GetSuccessValue<T>(T crit, T hit, T miss)
    {
        if (TempFlags.IsSet(EffectTempFlags.Crit))
        {
            return crit;
        }
        else if (TempFlags.IsSet(EffectTempFlags.Hit))
        {
            return hit;
        }

        return miss;
    }

    public FinalValues GetFinalValues()
    {
        int damage = 0;
        int heal = 0;
        int absorb = 0;

        for (int i = 0; i < Effects.Count; ++i)
        {
            if (Effects[i] is not EffectTypes.DealElement e)
            {
                continue;
            }

            if (e.IsDamage)
            {
                damage += e.FinalValue;
            }
            else
            {
                heal += e.FinalValue;
            }

            if (e.AbsorbValue > 0)
            {
                absorb += e.AbsorbValue;
            }
        }

        return new FinalValues(damage, heal, absorb);
    }

    private void TriggerStage(ProcessStages stage)
    {
        if (_stageFlags.HasFlag(stage))
        {
            return;
        }

        _stageFlags |= stage;
        User.ProcessOutgoingEffect(this, stage);

        if (stage != ProcessStages.Resolved)
        {
            Target.ProcessIncomingEffect(this, stage);
        }
    }
}
