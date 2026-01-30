using System;

[Serializable]
public class CharacterConfig
{
    public TeamId TeamId;
    public CharacterTraits Traits;
    public ModelConfig Model;
    public int Exp;

    public StatConfig[] Stats;
    public TalentData[] Talents;
    public WeaponIds Weapon; // TODO - move to inventory config

    /* TODO
     * - inventory config
    */
}
