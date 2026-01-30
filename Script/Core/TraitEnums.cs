using System;

[Flags]
public enum CharacterTraits
{
    None = 0,

    Human = Bits.Bit0,
    Undead = Bits.Bit1,
    Giant = Bits.Bit2,
    Goblin = Bits.Bit3,
}

[Flags]
public enum GeneralTraits
{
    None = 0,

    Attack = Bits.Bit0,
    Unarmed = Bits.Bit1,
    Weapon = Bits.Bit2,
    Spell = Bits.Bit3,
    Enchantment = Bits.Bit4,
    Potent = Bits.Bit5, // can crit
    Blockable = Bits.Bit6, // can't be blocked
}
