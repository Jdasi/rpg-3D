
public enum StatTypes
{
    Invalid = -1,

    // core
    Vitality,
    Might,
    Finesse,
    Intellect,
    Presence,

    // defense
    ArmorClass,
    Reflex,
    Fortitude,
    Psyche,

    // luck
    HitChance,
    CritChance,
    BlockChance,

    RESERVED_ELEMENT_RESIST = 100,
    RESERVED_ELEMENT_BOOST = 200,
}

public static class StatHelpers
{
    public static StatTypes ToElementResistStat(this ElementTypes element)
    {
        return (StatTypes)(element + (int)StatTypes.RESERVED_ELEMENT_RESIST);
    }

    public static StatTypes ToElementBoostStat(this ElementTypes element)
    {
        return (StatTypes)(element + (int)StatTypes.RESERVED_ELEMENT_BOOST);
    }
}
