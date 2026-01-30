using System;

public enum SkillIds
{
    Invalid = 0,

    Attack,
    CrusaderStrike,
    FlurryOfBlows,
    Shockwave,
    ColossalArc,
    Fireball,
    WideSlash,
    Blindside,
    DoubleStrike,
    SwiftStrike,
    RecklessStrike,
    AerialBoomerang,
    ConeOfCold,
    Rebuke,
    Thunderwave,
    DivineStorm,
    SolarRay,
    RushAttack,
    CascadeBolt,
    ForceTunnel,
    WhirlwindStrike,
    Quickstep,
    Teleport,
    SacredShield,
    Heal,

    // TODO ..
    Flare,
    HalfSwordStrike,
    Lacerate,
    Impale,
    HeavySlash,
    DefensiveStrike,
    Rally,
    SparkingStrike,
    TB_FireSlash,
    TB_OverchargeFire,
    TB_FlameBurst,
    TB_Blastwave,
    TB_IceSlash,
    TB_OverchargeCold,
    TB_ColdMid,
    TB_ColdFinal,
    TB_LightningSlash,
    TB_OverchargeLightning,
    TB_LightningMid,
    TB_LightningBolt,
    TB_ThunderSlash,
    TB_OverchargeThunder,
    TB_Thunderwave,
    TB_ThunderFinal,

    //GuardBreak
    //Perseverence
    //Bless
    //Rally
    //DivineFavor
    //Maim
    //HealingSurge
    //MagicBurstArrow
    //Assassinate
    //Evasion
    //PiercingArrow
    //EtherSpear
    //Glimpse
    //MageWard
    //DistorionHammer
    //PurgingStrike
    //SwordBurst
    //Evocation
    //RushOfBattle
    //ShoulderBash
    //Overwhelm
    //SecondWind
    //Provoke
    //Determination
    //GutPunch
    //MindSpike
    //WitchBolt
    //Vortex
    //Blink
    //TensionShard
    //Slow
    //Hex
    //Enthral
    //Scorch
    //Ignition
    //FlameArc
    //BlastWave
    //LivingBomb
    //BlazingSpeed
    //ChaosMeteor
    //HammerShot
    //ConcussiveShot
    //ForceGrenade
    //GroundBurst
    //Resupply
    //IncendiaryShot
    //Bud-EE
    //Pulse
    //LightWave
    //RadiantBlast
    //ChainHeal
    //ChainLightning
    //SearingLight
    //Smite
    //DivineWrath
    //Illumination
    //DivineLance
    //IceLance
    //FireBlast
    //FrostNova
    //ArcaneExplosion
    //Execute
    //HammerOfWrath
    //PowerWordDeath
    //Renew
    //ArcaneBarrage
    //Swap
    //Ricochet
    //Adrenaline
}

public enum DefenseTypes
{
    None = 0,

    ArmorClass,
    Reflex,
    Fortitude,
    Psyche,
}

[Flags]
public enum TargetingFlags
{
    PRESET_NONE = 0,

    Enemy = Bits.Bit0,
    Ally = Bits.Bit1,
    User = Bits.Bit2,

    PRESET_FRIENDLY = User | Ally,
    PRESET_OTHERS = Enemy | Ally,
    PRESET_EVERYTHING = Enemy | Ally | User,
}

public enum ContextDataIds
{
    Targeting,
    TargetTokenPos,
    Tokens,
}

public enum SkillCategories
{
    Weapon,
    Career,
    Temporary,

    RESERVED_COUNT,
}
