using UnityEngine;

public static class WeaponDatabase
{
    private const int BASE_VALUE_LONGSWORD = 6;
    private const int BASE_VALUE_TECHSWORD = 3;

    public static Weapon Get(WeaponIds id)
    {
        Weapon weapon = GetInternal(id);
        Debug.Assert(weapon != null, $"[WeaponDatabase] Get - failed for id: {id}");
        weapon.Init(id);

        return weapon;

        static Weapon GetInternal(WeaponIds id)
        {
            return id switch
            {
                // ==============================================================================================================================================

                #region Longsword

                WeaponIds.Longsword_1 => new Weapon
                {
                    Type = WeaponTypes.Longsword,
                    Scaling = new WeaponScaling
                    {
                        Might = WeaponScaling.Ranks.C,
                        Finesse = WeaponScaling.Ranks.E,
                    },
                    Skills = context => new SkillIds[]
                    {
                        SkillIds.HalfSwordStrike,
                        SkillIds.Impale,
                    },
                    OnUse = context =>
                    {
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Slashing, BASE_VALUE_LONGSWORD, StatTypes.Might, StatTypes.Finesse);
                    },
                },

                WeaponIds.Longsword_Divine_1 => new Weapon
                {
                    Type = WeaponTypes.Longsword,
                    Scaling = new WeaponScaling
                    {
                        Might = WeaponScaling.Ranks.C,
                        Finesse = WeaponScaling.Ranks.E,
                        Presence = WeaponScaling.Ranks.D,
                    },
                    Skills = context => new SkillIds[]
                    {
                        SkillIds.HalfSwordStrike,
                        SkillIds.Heal,
                        SkillIds.Impale,
                    },
                    OnUse = context =>
                    {
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Slashing, BASE_VALUE_LONGSWORD - 2, StatTypes.Might, StatTypes.Finesse);
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Radiant, 1, StatTypes.Presence);
                    },
                },

                WeaponIds.Longsword_Heavy_1 => new Weapon
                {
                    Type = WeaponTypes.Longsword,
                    Scaling = new WeaponScaling
                    {
                        Might = WeaponScaling.Ranks.C_Plus,
                        Finesse = WeaponScaling.Ranks.F,
                    },
                    Skills = context => new SkillIds[]
                    {
                        SkillIds.HalfSwordStrike,
                        SkillIds.ColossalArc,
                    },
                    OnUse = context =>
                    {
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Slashing, BASE_VALUE_LONGSWORD + 2, StatTypes.Might, StatTypes.Finesse);
                    },
                },

                WeaponIds.Longsword_Heavy_2 => new Weapon
                {
                    Type = WeaponTypes.Longsword,
                    Scaling = new WeaponScaling
                    {
                        Might = WeaponScaling.Ranks.C_Plus,
                        Finesse = WeaponScaling.Ranks.F,
                    },
                    Skills = context => new SkillIds[]
                    {
                        SkillIds.ColossalArc,
                        SkillIds.HalfSwordStrike,
                        SkillIds.WideSlash,
                    },
                    OnUse = context =>
                    {
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Slashing, BASE_VALUE_LONGSWORD, StatTypes.Might);
                    },
                },

                WeaponIds.Longsword_Heavy_3 => new Weapon
                {
                    Type = WeaponTypes.Longsword,
                    Scaling = new WeaponScaling
                    {
                        Might = WeaponScaling.Ranks.B,
                    },
                    Skills = context => new SkillIds[]
                    {
                        SkillIds.ColossalArc,
                        SkillIds.HeavySlash,
                        SkillIds.WideSlash,
                    },
                    OnUse = context =>
                    {
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Slashing, BASE_VALUE_LONGSWORD + 6, StatTypes.Might);
                    },
                },

                WeaponIds.Longsword_Dex_1 => new Weapon
                {
                    Type = WeaponTypes.Longsword,
                    Scaling = new WeaponScaling
                    {
                        Might = WeaponScaling.Ranks.E,
                        Finesse = WeaponScaling.Ranks.C,
                    },
                    Skills = context => new SkillIds[]
                    {
                        SkillIds.Blindside,
                        SkillIds.DoubleStrike,
                        SkillIds.Lacerate,
                    },
                    OnUse = context =>
                    {
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Slashing, BASE_VALUE_LONGSWORD, StatTypes.Might, StatTypes.Finesse);
                    },
                },

                WeaponIds.Longsword_Dex_2 => new Weapon
                {
                    Type = WeaponTypes.Longsword,
                    Scaling = new WeaponScaling
                    {
                        Might = WeaponScaling.Ranks.F,
                        Finesse = WeaponScaling.Ranks.C_Plus,
                    },
                    Skills = context => new SkillIds[]
                    {
                        SkillIds.Blindside,
                        SkillIds.Lacerate,
                        SkillIds.SwiftStrike,
                    },
                    OnUse = context =>
                    {
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Slashing, BASE_VALUE_LONGSWORD - 2, StatTypes.Might, StatTypes.Finesse);
                    },
                },

                WeaponIds.Longsword_Gyro_1 => new Weapon
                {
                    Type = WeaponTypes.Longsword,
                    Scaling = new WeaponScaling
                    {
                        Might = WeaponScaling.Ranks.F,
                        Finesse = WeaponScaling.Ranks.B,
                    },
                    Skills = context => new SkillIds[]
                    {
                        SkillIds.DoubleStrike,
                        SkillIds.RushAttack,
                        SkillIds.SwiftStrike,
                    },
                    OnUse = context =>
                    {
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Slashing, BASE_VALUE_LONGSWORD - 4, StatTypes.Might, StatTypes.Finesse);
                    },
                },

                WeaponIds.Longsword_Fire_1 => new Weapon
                {
                    Type = WeaponTypes.Longsword,
                    Flags = Weapon.ConfigFlags.FinalEnchant,
                    Scaling = new WeaponScaling
                    {
                        Might = WeaponScaling.Ranks.C,
                        Finesse = WeaponScaling.Ranks.E,
                    },
                    Skills = context => new SkillIds[]
                    {
                        SkillIds.Flare,
                        SkillIds.HalfSwordStrike,
                        SkillIds.SparkingStrike,
                    },
                    OnUse = context =>
                    {
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Slashing, BASE_VALUE_LONGSWORD, StatTypes.Might, StatTypes.Finesse);
                    },
                    Enchantment = WeaponEnchantmentIds.Scorching,
                },

                WeaponIds.Longsword_Claw_1 => new Weapon
                {
                    Type = WeaponTypes.Longsword,
                    Scaling = new WeaponScaling
                    {
                        Might = WeaponScaling.Ranks.D_Plus,
                        Finesse = WeaponScaling.Ranks.D,
                    },
                    Skills = context => new SkillIds[]
                    {
                        SkillIds.Lacerate,
                        SkillIds.WideSlash,
                    },
                    OnUse = context =>
                    {
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Slashing, BASE_VALUE_LONGSWORD, StatTypes.Might, StatTypes.Finesse);

                        if (Rand.RollUnder(10))
                        {
                            context.Effects.Add(new EffectTypes.ApplyCondition(ConditionIds.Bleeding, 2));
                        }
                    },
                },

                WeaponIds.Longsword_Champion_1 => new Weapon
                {
                    Type = WeaponTypes.Longsword,
                    Scaling = new WeaponScaling
                    {
                        Might = WeaponScaling.Ranks.C,
                        Finesse = WeaponScaling.Ranks.E,
                    },
                    Skills = context => new SkillIds[]
                    {
                        SkillIds.DefensiveStrike,
                        SkillIds.Impale,
                        SkillIds.Rally,
                    },
                    OnUse = context =>
                    {
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Slashing, BASE_VALUE_LONGSWORD, StatTypes.Might, StatTypes.Finesse);
                    },
                },

                #endregion

                #region TechBlade

                WeaponIds.TechBlade_Fire => new Weapon
                {
                    Type = WeaponTypes.TechBlade,
                    Scaling = new WeaponScaling
                    {
                        Might = WeaponScaling.Ranks.D,
                        Finesse = WeaponScaling.Ranks.E_Plus,
                    },
                    Skills = context => TechBladeSkills(context.Quality,
                        SkillIds.TB_FireSlash,
                        SkillIds.TB_OverchargeFire,
                        SkillIds.TB_FlameBurst,
                        SkillIds.TB_Blastwave),
                    OnUse = context =>
                    {
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Slashing, BASE_VALUE_TECHSWORD, StatTypes.Might, StatTypes.Finesse);
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Fire, BASE_VALUE_TECHSWORD);
                    },
                },

                WeaponIds.TechBlade_Cold => new Weapon
                {
                    Type = WeaponTypes.TechBlade,
                    Scaling = new WeaponScaling
                    {
                        Might = WeaponScaling.Ranks.D,
                        Finesse = WeaponScaling.Ranks.E_Plus,
                    },
                    Skills = context => TechBladeSkills(context.Quality,
                        SkillIds.TB_IceSlash,
                        SkillIds.TB_OverchargeCold,
                        SkillIds.TB_ColdMid,
                        SkillIds.TB_ColdFinal),
                    OnUse = context =>
                    {
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Slashing, BASE_VALUE_TECHSWORD, StatTypes.Might, StatTypes.Finesse);
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Cold, BASE_VALUE_TECHSWORD);
                    },
                },

                WeaponIds.TechBlade_Lightning => new Weapon
                {
                    Type = WeaponTypes.TechBlade,
                    Scaling = new WeaponScaling
                    {
                        Might = WeaponScaling.Ranks.D,
                        Finesse = WeaponScaling.Ranks.E_Plus,
                    },
                    Skills = context => TechBladeSkills(context.Quality,
                        SkillIds.TB_LightningSlash,
                        SkillIds.TB_OverchargeLightning,
                        SkillIds.TB_LightningMid,
                        SkillIds.TB_LightningBolt),
                    OnUse = context =>
                    {
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Slashing, BASE_VALUE_TECHSWORD, StatTypes.Might, StatTypes.Finesse);
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Lightning, BASE_VALUE_TECHSWORD);
                    },
                },

                WeaponIds.TechBlade_Thunder => new Weapon
                {
                    Type = WeaponTypes.TechBlade,
                    Scaling = new WeaponScaling
                    {
                        Might = WeaponScaling.Ranks.D,
                        Finesse = WeaponScaling.Ranks.E_Plus,
                    },
                    Skills = context => TechBladeSkills(context.Quality,
                        SkillIds.TB_ThunderSlash,
                        SkillIds.TB_OverchargeThunder,
                        SkillIds.TB_Thunderwave,
                        SkillIds.TB_ThunderFinal),
                    OnUse = context =>
                    {
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Slashing, BASE_VALUE_TECHSWORD, StatTypes.Might, StatTypes.Finesse);
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Thunder, BASE_VALUE_TECHSWORD);
                    },
                },

                #endregion

                #region ENEMY

                WeaponIds.ENEMY_GENERIC => new Weapon
                {
                    Type = WeaponTypes.ENEMY_GENERIC,
                    Scaling = new WeaponScaling
                    {
                        Might = WeaponScaling.Ranks.B,
                        Finesse = WeaponScaling.Ranks.B,
                    },
                    Skills = null,
                    OnUse = context =>
                    {
                        context.Effects.AddWeaponScaledElement(context.User, ElementTypes.Slashing, 3, StatTypes.Might, StatTypes.Finesse);
                    },
                },

                #endregion // ENEMY

                // ==============================================================================================================================================
                _ => null,
            };

            static SkillIds[] TechBladeSkills(WeaponScaling.QualityLevels quality, SkillIds skill1, SkillIds skill2, SkillIds temperedSkill, SkillIds masterworkSkill)
            {
                SkillIds[] ids = new SkillIds[quality switch
                {
                    WeaponScaling.QualityLevels.Tier5_Masterwork => 4,
                    WeaponScaling.QualityLevels.Tier3_Tempered => 3,
                    _ => 2,
                }];

                ids[0] = skill1;
                ids[1] = skill2;

                if (quality >= WeaponScaling.QualityLevels.Tier3_Tempered)
                {
                    ids[2] = temperedSkill;
                }

                if (quality >= WeaponScaling.QualityLevels.Tier5_Masterwork)
                {
                    ids[3] = masterworkSkill;
                }

                return ids;
            }
        }
    }
}
