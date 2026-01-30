
public class WeaponScaling
{
    public enum QualityLevels
    {
        Tier1_Crude,
        Tier2_Wrought,
        Tier3_Tempered,
        Tier4_Exquisite,
        Tier5_Masterwork,
    }

    public enum Ranks
    {
        None,

        S_Plus,
        S,
        A_Plus,
        A,
        B_Plus,
        B,
        C_Plus,
        C,
        D_Plus,
        D,
        E_Plus,
        E,
        F_Plus,
        F,
    }

    public int MaxInfusionLevel { get; init; } = 10;
    public Ranks Might { get; init; }
    public Ranks Finesse { get; init; }
    public Ranks Intellect { get; init; }
    public Ranks Presence { get; init; }
}
