using System;

public sealed class Weapon
{
    public enum ConfigFlags
    {
        None = 0,

        FinalScaling = Bits.Bit0,
        FinalEnchant = Bits.Bit1,
        SpellHit = Bits.Bit2,
    }

    public struct SkillsGetterContext
    {
        public CharacterData User;
        public WeaponScaling.QualityLevels Quality;
    }

    public WeaponTypes Type { get; init; }
    public WeaponIds WeaponId { get; private set; }
    public ConfigFlags Flags { get; init; }
    public WeaponScaling Scaling { get; init; }

    public delegate SkillIds[] SkillsGetter(SkillsGetterContext context);
    public SkillsGetter Skills { get; init; }

    public delegate void ProcessContextHandler(EffectResolveContext context);
    public ProcessContextHandler OnUse { private get; init; }
    public ProcessContextHandler PostUse { private get; init; }

    public WeaponScaling.QualityLevels Quality { get; set; }
    public WeaponEnchantmentIds Enchantment { get; set; }
    public int InfusionLevel { get; set; }

    public void Init(WeaponIds id)
    {
        WeaponId = id;
    }

    public void ProcessOutgoingEffect(EffectResolveContext context, EffectResolveContext.ProcessStages stage)
    {
        bool isWeaponSkill = context.Traits.HasFlag(GeneralTraits.Weapon);
        bool shouldApplyUpgrade = ShouldApplyUpgrade();

        switch (stage)
        {
            case EffectResolveContext.ProcessStages.BeforeTest:
            {
                if (shouldApplyUpgrade)
                {
                    context.Effects.Add(new EffectTypes.HitChance(InfusionLevel));
                }
            } break;

            case EffectResolveContext.ProcessStages.AfterTest:
            {
                if (isWeaponSkill)
                {
                    OnUse(context);
                }

                if (shouldApplyUpgrade)
                {
                    for (int i = 0; i < context.Effects.Count; ++i)
                    {
                        if (context.Effects[i] is not EffectTypes.DealElement dealElement)
                        {
                            continue;
                        }

                        dealElement.BaseValue += InfusionLevel;
                        dealElement.FinalValue += InfusionLevel;
                    }
                }
            } break;

            case EffectResolveContext.ProcessStages.Resolved:
            {
                if (isWeaponSkill)
                {
                    PostUse?.Invoke(context);
                }
            } break;
        }

        if (Enchantment != WeaponEnchantmentIds.Invalid)
        {
            WeaponEnchantments.ProcessOutgoingEffect(Enchantment, context, stage);
        }

        return;

        bool ShouldApplyUpgrade()
        {
            if (InfusionLevel <= 0)
            {
                return false;
            }

            if (context.Traits.HasFlag(GeneralTraits.Weapon))
            {
                return true;
            }

            if (context.Traits.HasFlag(GeneralTraits.Spell)
                && Flags.HasFlag(ConfigFlags.SpellHit))
            {
                return true;
            }

            return false;
        }
    }

    public int CalculateScaling(int baseValue)
    {
        return (int)Math.Ceiling(baseValue * Quality switch
        {
            WeaponScaling.QualityLevels.Tier1_Crude         => 1.00f,
            WeaponScaling.QualityLevels.Tier2_Wrought       => 1.50f,
            WeaponScaling.QualityLevels.Tier3_Tempered      => 2.00f,
            WeaponScaling.QualityLevels.Tier4_Exquisite     => 2.50f,
            WeaponScaling.QualityLevels.Tier5_Masterwork    => 3.00f,

            _ => 0,
        });
    }

    public int CalculateScaling(CharacterData user, StatTypes stat)
    {
        WeaponScaling.Ranks rank = stat switch
        {
            StatTypes.Might => Scaling.Might,
            StatTypes.Finesse => Scaling.Finesse,
            StatTypes.Intellect => Scaling.Intellect,
            StatTypes.Presence => Scaling.Presence,

            _ => WeaponScaling.Ranks.None,
        };

        if (rank == WeaponScaling.Ranks.None)
        {
            return 0;
        }

        if (Quality > WeaponScaling.QualityLevels.Tier1_Crude
            && !Flags.HasFlag(ConfigFlags.FinalScaling))
        {
            int diff = Quality - WeaponScaling.QualityLevels.Tier1_Crude;
            int newRankValue = Math.Clamp((int)rank - diff, (int)WeaponScaling.Ranks.S_Plus, (int)WeaponScaling.Ranks.E);
            rank = (WeaponScaling.Ranks)newRankValue;
        }

        return (int)Math.Ceiling(user.Stats.Get(stat) * rank switch
        {
            WeaponScaling.Ranks.S_Plus    => 2.00f,
            WeaponScaling.Ranks.S         => 1.90f,
            WeaponScaling.Ranks.A_Plus    => 1.70f,
            WeaponScaling.Ranks.A         => 1.60f,
            WeaponScaling.Ranks.B_Plus    => 1.40f,
            WeaponScaling.Ranks.B         => 1.30f,
            WeaponScaling.Ranks.C_Plus    => 1.10f,
            WeaponScaling.Ranks.C         => 1.00f,
            WeaponScaling.Ranks.D_Plus    => 0.80f,
            WeaponScaling.Ranks.D         => 0.70f,
            WeaponScaling.Ranks.E_Plus    => 0.50f,
            WeaponScaling.Ranks.E         => 0.40f,
            WeaponScaling.Ranks.F_Plus    => 0.20f,
            WeaponScaling.Ranks.F         => 0.10f,

            _ => 0,
        });
    }
}
