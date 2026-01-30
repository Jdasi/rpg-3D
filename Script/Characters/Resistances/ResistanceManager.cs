using System.Collections.Generic;
using UnityEngine;

public class ResistanceManager
{
    private List<BarrierInstance> _barriers;

    private readonly CharacterData _owner;
    private BarrierHandle _nextHandle;

    public ResistanceManager(CharacterData owner)
    {
        _owner = owner;
    }

    public bool HasBarrier()
    {
        return _barriers?.Count > 0;
    }

    public BarrierHandle AddBarrier(int priority, int health, ElementTypes types)
    {
        _barriers ??= new List<BarrierInstance>();
        _barriers.Add(new BarrierInstance(_nextHandle, priority, health, types));
        _barriers.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        return _nextHandle++;
    }

    public void RemoveBarrier(BarrierHandle handle)
    {
        int index = FindBarrierIndex(handle);

        if (index < 0)
        {
            return;
        }

        if (_barriers.Count == 1)
        {
            _barriers = null;
        }
        else
        {
            _barriers.RemoveAt(index);
        }
    }

    public void SetBarrierHealth(BarrierHandle handle, int health)
    {
        int index = FindBarrierIndex(handle);

        if (index >= 0)
        {
            _barriers[index].Health = health;
        }
    }

    public void AddBarrierHealth(BarrierHandle handle, int health)
    {
        int index = FindBarrierIndex(handle);

        if (index >= 0)
        {
            _barriers[index].Health += health;
        }
    }

    public void ResolveIncomingEffect(EffectResolveContext context)
    {
        ResolveBarriers(context);
        ResolveResistance(context);
    }

    private void ResolveBarriers(EffectResolveContext context)
    {
        if (_barriers == null)
        {
            return;
        }

        for (int barrierIndex = 0; barrierIndex < _barriers.Count; ++barrierIndex)
        {
            BarrierInstance barrier = _barriers[barrierIndex];

            for (int elementIndex = 0; elementIndex < context.Effects.Count; ++elementIndex)
            {
                if (context.Effects[elementIndex] is not EffectTypes.DealElement element)
                {
                    continue;
                }

                if (!barrier.AbsorbTypes.HasFlag(element.Type))
                {
                    continue;
                }

                context.TempFlags.Set(EffectTempFlags.Absorbed, true);

                int prevHealth = barrier.Health;
                barrier.Health = Mathf.Max(0, barrier.Health - element.FinalValue);
                int diff = prevHealth - barrier.Health;

                element.FinalValue -= diff;
                element.AbsorbValue += diff;

                if (barrier.Health == 0)
                {
                    LocalEvents.BarrierBroken.Invoke(new LocalEventData.BarrierBroken
                    {
                        Handle = barrier.Handle,
                        Source = context.Source,
                    });

                    break;
                }
            }
        }
    }

    private void ResolveResistance(EffectResolveContext context)
    {
        for (int i = 0; i < context.Effects.Count; ++i)
        {
            if (context.Effects[i] is not EffectTypes.DealElement element)
            {
                continue;
            }

            StatTypes resistStat = element.Type.ToElementResistStat();
            int resist = _owner.Stats.Get(resistStat);

            if (resist <= 0)
            {
                continue;
            }

            float resistFactor = resist / 100f;
            element.FinalValue -= (int)(element.FinalValue * resistFactor);
        }
    }

    private int FindBarrierIndex(BarrierHandle handle)
    {
        for (int i = 0; i < _barriers?.Count; ++i)
        {
            if (_barriers[i].Handle == handle)
            {
                return i;
            }
        }

        return -1;
    }
}
