using System.Collections;
using System.Collections.Generic;

public abstract class Skill
{
    public SkillIds Id { get; private set; }
    public GeneralTraits Traits { get; init; }

    public delegate SkillCostConfig SkillCostConfigGetter(CharacterData user);
    public SkillCostConfigGetter CostConfig { get; init; }

    public delegate void ProcessTargetHandler(SkillContext context);
    public ProcessTargetHandler ProcessTarget { protected get; init; }

    public delegate void PreResolveHandler(SkillContext context);
    public PreResolveHandler PreResolve { protected get; init; }

    public delegate void PostResolveHandler(SkillContext context);
    public PostResolveHandler PostResolve { protected get; init; }

    public void Init(SkillIds id)
    {
        Id = id;
    }

    public abstract ITargetingConfig CreateTargetingConfig(CharacterData user);
    public abstract ITargetingSystem CreateTargetingSystem(SkillContext context);
    public abstract IEnumerator Resolve(SkillContext context);

    protected void ProcessTargets(SkillContext context, List<Token> tokens = null)
    {
        if (context.Target != null)
        {
            context.Put(ContextDataIds.TargetTokenPos, context.Target.transform.position);
            ProcessTarget?.Invoke(context);
        }

        if (tokens != null)
        {
            context.Put(ContextDataIds.Tokens, tokens);

            for (int i = 0; i < tokens.Count; ++i)
            {
                context.Target = tokens[i];
                ProcessTarget?.Invoke(context);
            }
        }
    }
}

public abstract class Skill<TTargetingSystem, TTargetingConfig> : Skill
    where TTargetingSystem : ITargetingSystem, new()
    where TTargetingConfig : ITargetingConfig
{
    public delegate TTargetingConfig TargetingConfigGetter(CharacterData user);
    public TargetingConfigGetter TargetingConfig { init => _targetingConfig = value; }

    private TargetingConfigGetter _targetingConfig;

    public sealed override ITargetingConfig CreateTargetingConfig(CharacterData user)
    {
        return CreateTargetingConfigInternal(user);
    }

    public sealed override ITargetingSystem CreateTargetingSystem(SkillContext context)
    {
        return CreateTargetingSystemInternal(context);
    }

    public sealed override IEnumerator Resolve(SkillContext context)
    {
        LocalEvents.SkillStarted.Invoke(new LocalEventData.SkillStarted
        {
            SkillId = Id,
            CharacterId = context.User.Data.CharacterId,
        });

        using (context.Tracker = new SkillEffectTracker(Id))
        {
            TTargetingSystem targeting = CreateTargetingSystemInternal(context);
            context.Put(ContextDataIds.Targeting, targeting);

            yield return OnPreResolve(context, targeting);
            PreResolve?.Invoke(context);

            SkillCostConfig skillCost = CostConfig?.Invoke(context.User.Data);
            context.User.Data.ProcessSkillCostConfig(Id, skillCost, true);

            yield return BoardControl.SetTokenReactionsEnabled(true);
            yield return OnResolve(context, targeting);
            yield return BoardControl.SetTokenReactionsEnabled(false);

            yield return OnPostResolve(context, targeting);
            PostResolve?.Invoke(context);
        }

        LocalEvents.SkillFinished.Invoke(new LocalEventData.SkillFinished
        {
            SkillId = Id,
            CharacterId = context.User.Data.CharacterId,
            Tracker = context.Tracker,
            Traits = Traits,
        });
    }

    protected virtual IEnumerator OnPreResolve(SkillContext context, TTargetingSystem targeting) { yield break; }
    protected abstract IEnumerator OnResolve(SkillContext context, TTargetingSystem targeting);
    protected virtual IEnumerator OnPostResolve(SkillContext context, TTargetingSystem targeting) { yield break; }

    private TTargetingConfig CreateTargetingConfigInternal(CharacterData user)
    {
        TTargetingConfig config = _targetingConfig.Invoke(user);
        user.ProcessTargetingConfig(Id, config);
        return config;
    }

    private TTargetingSystem CreateTargetingSystemInternal(SkillContext context)
    {
        TTargetingSystem targeting = new();
        TTargetingConfig config = CreateTargetingConfigInternal(context.User.Data);
        targeting.Init(context, config);
        return targeting;
    }
}
