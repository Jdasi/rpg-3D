using System;
using System.Collections.Generic;

public class StatBlock
{
    public delegate int Modifier(int value);

    public int MaxHealth { get; private set; }
    public int Health { get; private set; }
    public float HealthPercent => MaxHealth > 0 ? Health / (float)MaxHealth : 0;

    private readonly CharacterId _characterId;
    private readonly Dictionary<StatTypes, int> _stats;

    private Dictionary<StatTypes, List<Modifier>> _modifiers;

    public StatBlock(CharacterId characterId)
    {
        _characterId = characterId;
        _stats = new Dictionary<StatTypes, int>();
    }

    public int Get(StatTypes stat)
    {
        if (!_stats.TryGetValue(stat, out int value))
        {
            value = Default();
        }

        if (_modifiers != null
            && _modifiers.TryGetValue(stat, out List<Modifier> modifierList))
        {
            for (int i = 0; i < modifierList.Count; ++i)
            {
                value = modifierList[i](value);
            }
        }

        return Math.Max(Min(), value);

        int Default() => stat switch
        {
            StatTypes.Vitality => 1,
            StatTypes.Might => 1,
            StatTypes.Finesse => 1,
            StatTypes.Intellect => 1,
            StatTypes.Presence => 1,

            StatTypes.ArmorClass => (int)Math.Ceiling((float)Get(StatTypes.Finesse) / 2),
            StatTypes.Fortitude => Defense(StatTypes.Vitality, StatTypes.Might),
            StatTypes.Reflex => Defense(StatTypes.Finesse, StatTypes.Intellect),
            StatTypes.Psyche => Defense(StatTypes.Intellect, StatTypes.Presence),

            StatTypes.HitChance => 85,
            StatTypes.CritChance => 5,

            _ => 0,
        };

        int Min() => stat switch
        {
            StatTypes.Vitality or
            StatTypes.Might or
            StatTypes.Finesse or
            StatTypes.Intellect or
            StatTypes.Presence => 1,

            _ => 0,
        };

        int Defense(StatTypes stat1, StatTypes stat2)
        {
            return (int)Math.Ceiling((float)(Get(stat1) + Get(stat2)) / 2);
        }
    }

    public void Set(StatTypes stat, int value)
    {
        if (value > 0)
        {
            _stats[stat] = value;
        }
        else
        {
            _stats.Remove(stat);
        }

        OnStatChanged(stat);
    }

    public void AddModifier(StatTypes stat, Modifier modifier)
    {
        _modifiers ??= new Dictionary<StatTypes, List<Modifier>>();

        if (!_modifiers.TryGetValue(stat, out List<Modifier> list))
        {
            list = _modifiers[stat] = new List<Modifier>();
        }

        list.Add(modifier);
        OnStatChanged(stat);
    }

    public void RemoveModifier(StatTypes stat, Modifier modifier)
    {
        if (_modifiers == null)
        {
            return;
        }

        if (!_modifiers.TryGetValue(stat, out List<Modifier> list))
        {
            return;
        }

        list.Remove(modifier);

        if (list.Count == 0)
        {
            if (_modifiers.Count > 1)
            {
                _modifiers.Remove(stat);
            }
            else
            {
                _modifiers = null;
            }
        }

        OnStatChanged(stat);
    }

    public void SetModifier(StatTypes stat, Modifier modifier, bool set)
    {
        if (set)
        {
            AddModifier(stat, modifier);
        }
        else
        {
            RemoveModifier(stat, modifier);
        }
    }

    public void SetHealth(int amount)
    {
        if (Health == amount)
        {
            return;
        }

        int prevHealth = Health;
        Health = Math.Clamp(amount, 0, MaxHealth);

        if (Health != prevHealth)
        {
            LocalEvents.CharacterHealthChanged.Invoke(new LocalEventData.CharacterHealthChanged
            {
                CharacterId = _characterId,
                Health = Health,
                MaxHealth = MaxHealth,
            });
        }
    }

    public void AdjustHealth(int amount)
    {
        SetHealth(Health + amount);
    }

    public void ResetHealth()
    {
        RecalculateMaxHealth();
        SetHealth(MaxHealth);
    }

    private void RecalculateMaxHealth()
    {
        int prevMaxHealth = MaxHealth;
        MaxHealth = Get(StatTypes.Vitality) * 2;

        if (MaxHealth != prevMaxHealth)
        {
            LocalEvents.CharacterHealthChanged.Invoke(new LocalEventData.CharacterHealthChanged
            {
                CharacterId = _characterId,
                Health = Health,
                MaxHealth = MaxHealth,
            });
        }
    }

    private void OnStatChanged(StatTypes stat)
    {
        if (stat == StatTypes.Vitality)
        {
            RecalculateMaxHealth();
        }
    }
}
