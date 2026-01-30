using System;

public class SkillEffectTracker : IDisposable
{
    private readonly SkillIds _skillId;

    /// <summary> All damage dealt by this usage of the skill. </summary>
    public int TotalDamage { get; private set; }

    /// <summary> All healing dealt by this usage of the skill. </summary>
    public int TotalHeal { get; private set; }

    public SkillEffectTracker(SkillIds id)
    {
        _skillId = id;

        LocalEvents.CharacterHealthEffect.Subscribe(OnCharacterHealthEffect);
    }

    public void Dispose()
    {
        LocalEvents.CharacterHealthEffect.Unsubscribe(OnCharacterHealthEffect);
    }

    private bool ValidateSource(EffectSourceInfo source)
    {
        if (source == null)
        {
            return false;
        }

        if (source.SkillId != _skillId)
        {
            return false;
        }

        return true;
    }

    private void OnCharacterHealthEffect(LocalEventData.CharacterHealthEffect data)
    {
        if (!ValidateSource(data.Source))
        {
            return;
        }

        if (data.IsDamage)
        {
            TotalDamage += data.Amount;
        }
        else
        {
            TotalHeal += data.Amount;
        }
    }
}
