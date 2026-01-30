
public enum ElementTypes
{
    PRESET_INVALID = 0,

    Bludgeoning = Bits.Bit0,
    Piercing = Bits.Bit1,
    Slashing = Bits.Bit2,
    Acid = Bits.Bit3,
    Cold = Bits.Bit4,
    Lightning = Bits.Bit5,
    Fire = Bits.Bit6,
    Thunder = Bits.Bit7,
    Force = Bits.Bit8,
    Necrotic = Bits.Bit9,
    Radiant = Bits.Bit10,
    Psychic = Bits.Bit11,
    Poison = Bits.Bit12,
    Bleed = Bits.Bit13,
    Restoration = Bits.Bit14,

    PRESET_ALL_DAMAGE = PRESET_EVERYTHING & ~Restoration,
    PRESET_ELEMENTAL = Cold | Lightning | Fire | Thunder,
    PRESET_ARCANE = Force | Necrotic | Radiant | Psychic,
    PRESET_EVERYTHING = -1,
}
