
public static class WeaponEnchantments
{
    public static void ProcessApplied(WeaponEnchantmentIds enchantment, CharacterData user, bool applied)
    {
        switch (enchantment)
        {
            case WeaponEnchantmentIds.Fervor:
            {
                user.Stats.SetModifier(StatTypes.Might, value => value + 10, applied);
            } break;

            case WeaponEnchantmentIds.Alacrity:
            {
                user.Stats.SetModifier(StatTypes.Finesse, value => value + 10, applied);
            } break;

            case WeaponEnchantmentIds.Brilliance:
            {
                user.Stats.SetModifier(StatTypes.Intellect, value => value + 10, applied);
            } break;

            case WeaponEnchantmentIds.Sovereignty:
            {
                user.Stats.SetModifier(StatTypes.Presence, value => value + 10, applied);
            } break;

            case WeaponEnchantmentIds.TitansGrip:
            {
                user.Stats.SetModifier(StatTypes.Might, value => value + 5, applied);
            } break;

            case WeaponEnchantmentIds.WindFury:
            {
                user.Stats.SetModifier(StatTypes.Finesse, value => value + 5, applied);
            } break;

            case WeaponEnchantmentIds.MageCunning:
            {
                user.Stats.SetModifier(StatTypes.Intellect, value => value + 5, applied);
                user.SetCondition(ConditionIds.MageCunning, applied);
            } break;

            case WeaponEnchantmentIds.LordsCall:
            {
                user.Stats.SetModifier(StatTypes.Presence, value => value + 5, applied);
                user.SetCondition(ConditionIds.LordsCall_Aura, applied);
            } break;
        }
    }

    public static void ProcessOutgoingEffect(WeaponEnchantmentIds enchantment, EffectResolveContext context, EffectResolveContext.ProcessStages stage)
    {
        switch (enchantment)
        {
            case WeaponEnchantmentIds.Scorching:
            {
                if (stage != EffectResolveContext.ProcessStages.Resolved
                    || !context.Traits.HasFlag(GeneralTraits.Weapon)
                    || !context.TempFlags.IsSet(EffectTempFlags.Hit))
                {
                    return;
                }

                if (!Rand.RollUnder(20))
                {
                    return;
                }

                var resolveContext = EffectResolveContext.Create(GeneralTraits.Enchantment, context.User, context.Target);
                resolveContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Fire, 6));
                resolveContext.Resolve();
            } break;

            case WeaponEnchantmentIds.Eruption:
            {
                if (stage != EffectResolveContext.ProcessStages.Resolved
                    || !context.Traits.HasFlag(GeneralTraits.Weapon)
                    || !context.TempFlags.IsSet(EffectTempFlags.Crit))
                {
                    return;
                }

                var resolveContext = EffectResolveContext.Create(GeneralTraits.Enchantment, context.User, context.Target);
                resolveContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Fire, 12));
                resolveContext.Effects.Add(new EffectTypes.ApplyCondition(ConditionIds.Burning, 2));
                resolveContext.Resolve();
            } break;

            case WeaponEnchantmentIds.Salvation:
            {
                if (stage != EffectResolveContext.ProcessStages.Finalised)
                {
                    return;
                }

                context.Effects.MultiplyElement(ElementTypes.Restoration, 1.15f);
            } break;

            case WeaponEnchantmentIds.ElementalPower:
            {
                if (stage != EffectResolveContext.ProcessStages.Finalised)
                {
                    return;
                }

                context.Effects.MultiplyElements(ElementTypes.PRESET_ELEMENTAL, 1.15f);
            } break;

            case WeaponEnchantmentIds.ArcanePower:
            {
                if (stage != EffectResolveContext.ProcessStages.Finalised)
                {
                    return;
                }

                context.Effects.MultiplyElements(ElementTypes.PRESET_ARCANE, 1.15f);
            } break;

            case WeaponEnchantmentIds.Precision:
            {
                if (stage != EffectResolveContext.ProcessStages.BeforeTest
                    || !context.Traits.HasFlag(GeneralTraits.Weapon))
                {
                    return;
                }

                context.Effects.Add(new EffectTypes.HitChance(10));
                context.Effects.Add(new EffectTypes.CritChance(5));
            } break;

            case WeaponEnchantmentIds.Lethality:
            {
                if (stage != EffectResolveContext.ProcessStages.BeforeTest
                    || !context.Traits.HasFlag(GeneralTraits.Weapon))
                {
                    return;
                }

                context.Effects.Add(new EffectTypes.CritChance(15));
            } break;

            case WeaponEnchantmentIds.Draining:
            {
                if (stage != EffectResolveContext.ProcessStages.Resolved
                    || !context.Traits.HasFlag(GeneralTraits.Weapon)
                    || !context.TempFlags.IsSet(EffectTempFlags.Hit))
                {
                    return;
                }

                if (Rand.RollUnder(20))
                {
                    var resolveContext = EffectResolveContext.Create(GeneralTraits.Enchantment, context.User, context.Target);
                    resolveContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Necrotic, 3));
                    resolveContext.Resolve();
                }

                if (context.Target.Stats.HealthPercent == 0)
                {
                    var healContext = EffectResolveContext.Create(GeneralTraits.Enchantment, context.User, context.User);
                    healContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Restoration, (int)(context.User.Stats.MaxHealth * 0.05f), 0));
                    healContext.Resolve();
                }
            } break;

            case WeaponEnchantmentIds.TitansGrip:
            {
                if (stage != EffectResolveContext.ProcessStages.Resolved
                    || !context.Traits.HasFlag(GeneralTraits.Weapon)
                    || !context.TempFlags.IsSet(EffectTempFlags.Hit))
                {
                    return;
                }

                if (!Rand.RollUnder(20))
                {
                    return;
                }

                var resolveContext = EffectResolveContext.Create(GeneralTraits.Enchantment, context.User, context.Target);
                resolveContext.Effects.Add(new EffectTypes.ApplyCondition(ConditionIds.Sundered, 2));
                resolveContext.Resolve();
            } break;

            case WeaponEnchantmentIds.WindFury:
            {
                if (stage != EffectResolveContext.ProcessStages.Resolved
                    || !context.Traits.HasFlag(GeneralTraits.Weapon)
                    || !context.TempFlags.IsSet(EffectTempFlags.Hit))
                {
                    return;
                }

                if (!Rand.RollUnder(10))
                {
                    return;
                }

                var resolveContext = EffectResolveContext.Create(GeneralTraits.Enchantment, context.User, context.Target);
                resolveContext.Effects.Add(new EffectTypes.DealElement(ElementTypes.Lightning, 4));
                resolveContext.Resolve();
            } break;
        }
    }
}
