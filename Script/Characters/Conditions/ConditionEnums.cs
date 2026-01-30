
public enum ConditionIds
{
    Invalid = 0,

    OffGuard,
    Bleeding,
    Burning,
    SacredShield,
    HolyPower,
    Sundered, // TODO - lower defense
    LordsCall_Aura, // TODO - aura: apply LordsCall to allies in range
    LordsCall, // TODO - +5 hit, +2 crit
    MageCunning // TODO - TP cost of spells reduced
}

public enum ConditionStackTypes
{
    Overwrite,
    Additive,
}
