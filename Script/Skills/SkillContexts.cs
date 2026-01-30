using System.Collections.Generic;
using UnityEngine;

public sealed class SkillContext
{
    public readonly SkillIds SkillId;
    public readonly GeneralTraits Traits;
    public readonly Token User;

    public Vector3 TargetPosition { get; private set; }
    public Vector3 CastDirection { get; private set; }

    // working data
    public Token Target;
    public SkillEffectTracker Tracker;
    private Dictionary<ContextDataIds, object> _data;

    public SkillContext(SkillIds id, GeneralTraits traits, Token user)
    {
        SkillId = id;
        User = user;
        Traits = traits;
    }

    public SkillContext(SkillIds id, GeneralTraits traits, Token user, FlatVector2 targetPosition)
        : this(id, traits, user)
    {
        SetTargetPosition(targetPosition);
    }

    public void SetTargetPosition(FlatVector2 position)
    {
        TargetPosition = position;
        CastDirection = User.transform.position.FlatDir(position);
    }

    public void Put(ContextDataIds id, object v)
    {
        _data ??= new Dictionary<ContextDataIds, object>();
        _data[id] = v;
    }

    public bool Get<T>(ContextDataIds id, out T v)
    {
        if (_data != null && _data.TryGetValue(id, out object o))
        {
            v = (T)o;
            return true;
        }

        v = default;
        return false;
    }
}
