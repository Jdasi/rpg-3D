using UnityEngine;

public static partial class SkillDatabase
{
    public const GeneralTraits DEFAULT_WEAPON_ATTACK_TRAITS = GeneralTraits.Attack | GeneralTraits.Weapon | GeneralTraits.Potent | GeneralTraits.Blockable;
    public const float BASE_MELEE_RANGE = 0f;

    public static Skill Get(SkillIds id)
    {
        Skill skill = GetInternal(id);
        Debug.Assert(skill != null, $"[SkillDatabase] Get - failed for id: {id}");
        skill.Init(id);

        return skill;

        static Skill GetInternal(SkillIds id)
        {
            return id switch
            {
                // ==============================================================================================================================================

                #region Target
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                SkillIds.Attack => new SkillTypes.Target
                {
                    Traits = DEFAULT_WEAPON_ATTACK_TRAITS,
                    TargetingConfig = user => new TargetingTypes.Target.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Range = BASE_MELEE_RANGE,
                    },
                    ProcessTarget = context =>
                    {
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.ArmorClass);
                        resolveContext.Resolve();
                    },
                },

                SkillIds.CrusaderStrike => new SkillTypes.Target
                {
                    Traits = DEFAULT_WEAPON_ATTACK_TRAITS,
                    TargetingConfig = user => new TargetingTypes.Target.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Range = BASE_MELEE_RANGE,
                    },
                    ProcessTarget = context =>
                    {
                        int bonusDamage = (int)(context.User.Data.Stats.Get(StatTypes.Presence) * 1.5f);
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.ArmorClass);
                        resolveContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Radiant, bonusDamage));
                        resolveContext.Resolve();
                    },
                    PostResolve = context =>
                    {
                        int healAmount = (int)(context.Tracker.TotalDamage * 0.5f);
                        var resolveContext = EffectResolveContext.Create(GeneralTraits.None, context.User.Data, context.User.Data, skillId: context.SkillId);
                        resolveContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Restoration, healAmount, 0));
                        resolveContext.Resolve();
                    },
                },

                SkillIds.FlurryOfBlows => new SkillTypes.Target
                {
                    Traits = DEFAULT_WEAPON_ATTACK_TRAITS,
                    TargetingConfig = user => new TargetingTypes.Target.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Range = BASE_MELEE_RANGE,
                    },
                    ProcessTarget = context =>
                    {
                        var primaryContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.ArmorClass);
                        primaryContext.Finalise();

                        var secondaryContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.ArmorClass);
                        secondaryContext.Finalise();

                        var combinedContext = EffectResolveContext.Combine(primaryContext, secondaryContext);
                        combinedContext.Resolve();
                    },
                },

                SkillIds.Blindside => new SkillTypes.Target
                {
                    Traits = DEFAULT_WEAPON_ATTACK_TRAITS,
                    TargetingConfig = user => new TargetingTypes.Target.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Range = BASE_MELEE_RANGE,
                    },
                    ProcessTarget = context =>
                    {
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data);

                        if (context.Target.Data.HasCondition(ConditionIds.OffGuard))
                        {
                            resolveContext.Effects.Add(new EffectTypes.CritChance(25));
                        }

                        resolveContext.DetermineSuccess(DefenseTypes.ArmorClass);

                        if (resolveContext.TempFlags.IsSet(EffectTempFlags.Crit))
                        {
                            resolveContext.Effects.Add(new EffectTypes.ApplyCondition(ConditionIds.Bleeding, 3));
                        }

                        resolveContext.Resolve();
                    },
                },

                SkillIds.DoubleStrike => new SkillTypes.Target
                {
                    Traits = DEFAULT_WEAPON_ATTACK_TRAITS,
                    TargetingConfig = user => new TargetingTypes.Target.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Range = BASE_MELEE_RANGE,
                    },
                    ProcessTarget = context =>
                    {
                        var primaryContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.ArmorClass);
                        primaryContext.Resolve();

                        // TODO - remove once animations are in
                        var secondaryContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.ArmorClass);
                        secondaryContext.Resolve();
                    },
                },

                SkillIds.SwiftStrike => new SkillTypes.Target
                {
                    Traits = DEFAULT_WEAPON_ATTACK_TRAITS,
                    TargetingConfig = user => new TargetingTypes.Target.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Range = BASE_MELEE_RANGE,
                    },
                    ProcessTarget = context =>
                    {
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.ArmorClass);
                        resolveContext.Effects.MultiplyElements(0.8f);
                        resolveContext.Resolve();
                    },
                },

                SkillIds.RecklessStrike => new SkillTypes.Target
                {
                    Traits = DEFAULT_WEAPON_ATTACK_TRAITS,
                    TargetingConfig = user => new TargetingTypes.Target.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Range = BASE_MELEE_RANGE,
                    },
                    ProcessTarget = context =>
                    {
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.ArmorClass);
                        resolveContext.Effects.MultiplyElements(1.25f);
                        resolveContext.Resolve();
                    },
                    PostResolve = context =>
                    {
                        var resolveContext = EffectResolveContext.Create(GeneralTraits.None, context.User.Data, context.User.Data, skillId: context.SkillId);
                        resolveContext.Effects.Add(new EffectTypes.ApplyCondition(ConditionIds.OffGuard, 1));
                        resolveContext.Resolve();
                    },
                },

                SkillIds.Rebuke => new SkillTypes.Target
                {
                    Traits = DEFAULT_WEAPON_ATTACK_TRAITS,
                    TargetingConfig = user => new TargetingTypes.Target.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Range = BASE_MELEE_RANGE,
                    },
                    ProcessTarget = context =>
                    {
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.ArmorClass);
                        float pushAmount = resolveContext.GetSuccessValue(7, 5, 3);
                        Vector3 pushDir = context.User.transform.position.FlatDir(context.Target.transform.position);

                        resolveContext.Effects.Add(new EffectTypes.AddForce(pushDir, pushAmount));
                        resolveContext.Resolve();
                    },
                },

                SkillIds.CascadeBolt => new SkillTypes.Target
                {
                    Traits = GeneralTraits.Spell | GeneralTraits.Potent,
                    TargetingConfig = user => new TargetingTypes.Target.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Range = 5f,
                        Radius = 3f,
                    },
                    ProcessTarget = context =>
                    {
                        var primaryContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.Reflex);
                        primaryContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Lightning, 8));
                        primaryContext.Resolve();

                        Token[] secondaryTargets = GameObject.FindObjectsByType<Token>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
                        context.Get(ContextDataIds.Targeting, out TargetingTypes.Target targeting);
                        context.Get(ContextDataIds.TargetTokenPos, out Vector3 targetTokenPos);

                        for (int i = 0; i < secondaryTargets.Length; ++i)
                        {
                            if (secondaryTargets[i] == context.Target)
                            {
                                continue;
                            }

                            if (!TargetingSystem.VerifyTarget(context.User.Data, secondaryTargets[i].Data, targeting.Config.Flags))
                            {
                                continue;
                            }

                            if (secondaryTargets[i].transform.position.FlatDist(targetTokenPos) > targeting.Config.Radius)
                            {
                                continue;
                            }

                            var secondaryContext = EffectResolveContext.Create(context, secondaryTargets[i].Data, DefenseTypes.Reflex);
                            secondaryContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Lightning, 4));
                            secondaryContext.Resolve();
                        }
                    },
                },

                SkillIds.SacredShield => new SkillTypes.Target
                {
                    Traits = GeneralTraits.Spell,
                    TargetingConfig = user => new TargetingTypes.Target.TargetingConfig
                    {
                        Flags = TargetingFlags.PRESET_FRIENDLY,
                        Range = 5f,
                    },
                    ProcessTarget = context =>
                    {
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data);
                        resolveContext.Effects.Add(new EffectTypes.ApplyCondition(ConditionIds.SacredShield, 3));
                        resolveContext.Resolve();
                    },
                },

                SkillIds.Heal => new SkillTypes.Target
                {
                    Traits = GeneralTraits.Spell | GeneralTraits.Potent,
                    TargetingConfig = user => new TargetingTypes.Target.TargetingConfig
                    {
                        Flags = TargetingFlags.PRESET_FRIENDLY,
                        Range = BASE_MELEE_RANGE,
                    },
                    ProcessTarget = context =>
                    {
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data);
                        resolveContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Restoration, 6));
                        resolveContext.Resolve();
                    },
                },
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                #endregion // Target

                #region Line
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                SkillIds.Shockwave => new SkillTypes.Line
                {
                    Traits = DEFAULT_WEAPON_ATTACK_TRAITS,
                    TargetingConfig = user => new TargetingTypes.Line.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Width = 1f,
                        Length = 3.5f,
                    },
                    ProcessTarget = context =>
                    {
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.ArmorClass);
                        resolveContext.Effects.MultiplyElements(0.6f);
                        resolveContext.Resolve();
                    },
                },

                SkillIds.Thunderwave => new SkillTypes.Line
                {
                    Traits = GeneralTraits.Potent | GeneralTraits.Spell,
                    TargetingConfig = user => new TargetingTypes.Line.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy | TargetingFlags.Ally,
                        Width = 4f,
                        Length = 4f,
                    },
                    ProcessTarget = context =>
                    {
                        int damage = (int)(context.User.Data.Stats.Get(StatTypes.Intellect) * 5f);
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.Fortitude);
                        resolveContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Thunder, damage));

                        float pushAmount = resolveContext.GetSuccessValue(7, 5, 3);
                        resolveContext.Effects.Add(new EffectTypes.AddForce(context.CastDirection, pushAmount));
                        resolveContext.Resolve();
                    },
                },
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                #endregion // Line

                #region Circle
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                SkillIds.Fireball => new SkillTypes.Circle
                {
                    Traits = GeneralTraits.Spell | GeneralTraits.Potent,
                    TargetingConfig = user => new TargetingTypes.Circle.TargetingConfig
                    {
                        Flags = TargetingFlags.User | TargetingFlags.Ally | TargetingFlags.Enemy,
                        Radius = 2.5f,
                        Range = 15f,
                    },
                    ProcessTarget = context =>
                    {
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.Reflex);
                        resolveContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Fire, 6));
                        resolveContext.Resolve();
                    },
                },

                SkillIds.DivineStorm => new SkillTypes.Circle
                {
                    Traits = DEFAULT_WEAPON_ATTACK_TRAITS,
                    TargetingConfig = user => new TargetingTypes.Circle.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Radius = 1.6f,
                    },
                    ProcessTarget = context =>
                    {
                        int bonusDamage = (int)(context.User.Data.Stats.Get(StatTypes.Presence) * 1.5f);
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.ArmorClass);
                        resolveContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Radiant, bonusDamage));
                        resolveContext.Resolve();

                    },
                    PostResolve = context =>
                    {
                        if (context.Tracker.TotalDamage == 0)
                        {
                            return;
                        }

                        int healAmount = (int)Mathf.Ceil(context.Tracker.TotalDamage * 0.35f);
                        var resolveContext = EffectResolveContext.Create(GeneralTraits.None, context.User.Data, context.User.Data, skillId: context.SkillId);
                        resolveContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Restoration, healAmount, 0));
                        resolveContext.Resolve();
                    },
                },
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                #endregion // Circle

                #region Cone
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                SkillIds.ConeOfCold => new SkillTypes.Cone
                {
                    Traits = GeneralTraits.Potent | GeneralTraits.Spell,
                    TargetingConfig = user => new TargetingTypes.Cone.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy | TargetingFlags.Ally,
                        Width = 6f,
                        Length = 5f,
                    },
                    ProcessTarget = context =>
                    {
                        int damage = (int)(context.User.Data.Stats.Get(StatTypes.Intellect) * 5f);
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.Fortitude);
                        resolveContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Cold, damage));
                        resolveContext.Resolve();
                    },
                },
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                #endregion // Cone

                #region Arc
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                SkillIds.WideSlash => new SkillTypes.Arc
                {
                    Traits = DEFAULT_WEAPON_ATTACK_TRAITS,
                    TargetingConfig = user => new TargetingTypes.Arc.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Radius = 1.75f,
                        Angle = 145f,
                    },
                    ProcessTarget = context =>
                    {
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.ArmorClass);
                        resolveContext.Effects.MultiplyElements(0.6f);
                        resolveContext.Resolve();
                    },
                },
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                #endregion // Arc

                #region Front Back
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                SkillIds.ColossalArc => new SkillTypes.FrontBack
                {
                    Traits = DEFAULT_WEAPON_ATTACK_TRAITS,
                    TargetingConfig = user => new TargetingTypes.FrontBack.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Width = 0.25f,
                        Length = 1.5f,
                    },
                    ProcessTarget = context =>
                    {
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.ArmorClass);
                        resolveContext.Effects.MultiplyElements(0.8f);
                        resolveContext.Resolve();
                    },
                },
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                #endregion // Front Back

                #region Line Spawn
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                SkillIds.AerialBoomerang => new SkillTypes.LineSpawn
                {
                    Traits = GeneralTraits.Potent,
                    TargetingConfig = user => new TargetingTypes.LineSpawn.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Width = 2f,
                        Length = 6f,
                    },
                    ProcessTarget = context =>
                    {
                        int damage = (int)(context.User.Data.Stats.Get(StatTypes.Intellect) * 5f);
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.Reflex);
                        resolveContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Bludgeoning, damage));
                        resolveContext.Resolve();
                    },
                },
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                #endregion // Line Spawn

                #region Ray
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                SkillIds.SolarRay => new SkillTypes.Ray
                {
                    Traits = GeneralTraits.Spell | GeneralTraits.Potent,
                    TargetingConfig = user => new TargetingTypes.Ray.TargetingConfig
                    {
                        Flags = TargetingFlags.User | TargetingFlags.Ally | TargetingFlags.Enemy,
                        MaxBounces = user.HasTalent(TalentIds.Augment_ImprovedSolarRay, out _) ? 2 : 1,
                        MaxAngle = 150f,
                    },
                    ProcessTarget = context =>
                    {
                        int damage = (int)(context.User.Data.Stats.Get(StatTypes.Presence) * 1.5f);
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.Reflex);
                        resolveContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Radiant, damage));
                        resolveContext.Resolve();
                    },
                },
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                #endregion // Ray

                #region Rush
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                SkillIds.RushAttack => new SkillTypes.Rush
                {
                    Traits = GeneralTraits.Spell | GeneralTraits.Potent | GeneralTraits.Weapon,
                    TargetingConfig = user => new TargetingTypes.Rush.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Range = 10f,
                    },
                    ProcessTarget = context =>
                    {
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.ArmorClass);
                        resolveContext.Resolve();
                    },
                },

                SkillIds.ForceTunnel => new SkillTypes.Rush
                {
                    Traits = GeneralTraits.Spell | GeneralTraits.Potent,
                    TargetingConfig = user => new TargetingTypes.Rush.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Range = 7f,
                        Penetrate = true,
                    },
                    ProcessTarget = context =>
                    {
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.Reflex);
                        resolveContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Force, 6));
                        resolveContext.Resolve();
                    },
                },

                SkillIds.Quickstep => new SkillTypes.Rush
                {
                    Traits = GeneralTraits.None,
                    TargetingConfig = user => new TargetingTypes.Rush.TargetingConfig
                    {
                        Range = 3f,
                        Penetrate = true,
                    },
                },
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                #endregion // Rush

                #region Tumble
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                SkillIds.WhirlwindStrike => new SkillTypes.Tumble
                {
                    Traits = DEFAULT_WEAPON_ATTACK_TRAITS,
                    TargetingConfig = user => new TargetingTypes.Tumble.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Range = 4f,
                        Width = 4f,
                    },
                    ProcessTarget = context =>
                    {
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.Reflex);
                        resolveContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Force, 6));
                        resolveContext.Resolve();
                    },
                },
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                #endregion // Tumble

                #region Teleport
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                SkillIds.Teleport => new SkillTypes.Teleport
                {
                    Traits = GeneralTraits.Spell,
                    TargetingConfig = user => new TargetingTypes.Teleport.TargetingConfig
                    {
                        Range = 10,
                    },
                },
                // ----------------------------------------------------------------------------------------------------------------------------------------------
                #endregion // Teleport

                // ==============================================================================================================================================
                _ => new SkillTypes.Target
                {
                    Traits = DEFAULT_WEAPON_ATTACK_TRAITS,
                    TargetingConfig = user => new TargetingTypes.Target.TargetingConfig
                    {
                        Flags = TargetingFlags.Enemy,
                        Range = BASE_MELEE_RANGE,
                    },
                    ProcessTarget = context =>
                    {
                        var resolveContext = EffectResolveContext.Create(context, context.Target.Data, DefenseTypes.ArmorClass);
                        resolveContext.Resolve();
                    },
                },
            };
        }
    }
}
