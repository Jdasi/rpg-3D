using System.Collections;
using System.Collections.Generic;

using static EffectTypes;

public class EffectCollection : IEnumerable<IEffectComponent>
{
    public int Count => _effects.Count;
    public IEffectComponent this[int index]
    {
        get { return _effects[index]; }
        set { _effects[index] = value; }
    }

    private readonly List<IEffectComponent> _effects;

    public EffectCollection()
    {
        _effects = new List<IEffectComponent>();
    }

    public EffectCollection(IEffectComponent effect)
    {
        _effects = new List<IEffectComponent>() { effect };
    }

    public EffectCollection(IEnumerable<IEffectComponent> effects)
    {
        _effects = new List<IEffectComponent>(effects);
    }

    public bool TryGet<T>(out T effect) where T : IEffectComponent
    {
        int index = _effects.FindIndex(elem => elem is T);

        if (index < 0)
        {
            effect = default;
            return false;
        }

        effect = (T)_effects[index];
        return true;
    }

    public void Add(IEnumerable<IEffectComponent> effects)
    {
        if (effects == null)
        {
            return;
        }

        foreach (var effect in effects)
        {
            Add(effect);
        }
    }

    public void Add(IEffectComponent effect)
    {
        switch (effect)
        {
            case DealElement e:
            {
                int index = _effects.FindIndex(elem => elem is DealElement existing && existing.Type == e.Type);

                if (index >= 0)
                {
                    DealElement existing = (DealElement)_effects[index];
                    existing.BaseValue += e.BaseValue;
                    existing.FinalValue += e.FinalValue;
                    return;
                }
            } break;

            case HitChance e:
            {
                int index = _effects.FindIndex(elem => elem is HitChance);

                if (index >= 0)
                {
                    ((HitChance)_effects[index]).Amount += e.Amount;
                    return;
                }
            } break;

            case CritChance e:
            {
                int index = _effects.FindIndex(elem => elem is CritChance);

                if (index >= 0)
                {
                    ((CritChance)_effects[index]).Amount += e.Amount;
                    return;
                }
            } break;

            case AddForce e:
            {
                int index = _effects.FindIndex(elem => elem is AddForce);

                if (index >= 0)
                {
                    ((AddForce)_effects[index]).Amount += e.Amount;
                    return;
                }
            } break;
        }

        _effects.Add(effect);
    }

    public void Remove(IEffectComponent effect)
    {
        _effects.Remove(effect);
    }

    public void Clear()
    {
        _effects.Clear();
    }

    public void Init(IEnumerable<IEffectComponent> effects)
    {
        Clear();
        Add(effects);
    }

    public IEnumerator<IEffectComponent> GetEnumerator()
    {
        return _effects.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
