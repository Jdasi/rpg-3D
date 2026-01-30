
public interface IConditionManager
{
    CharacterData Owner { get; }

    delegate void ProcessSkillConfigHandler(SkillIds skillId, SkillCostConfig config);
    event ProcessSkillConfigHandler OnProcessSkillCostConfig;

    delegate void ProcessTargetingConfigHandler(SkillIds skillId, ITargetingConfig config);
    event ProcessTargetingConfigHandler OnProcessTargetingConfig;

    delegate void ProcessIncomingEffectHandler(EffectResolveContext context, EffectResolveContext.ProcessStages stage);
    event ProcessIncomingEffectHandler OnProcessIncomingEffect;

    delegate void ProcessOutgoingEffectHandler(EffectResolveContext context, EffectResolveContext.ProcessStages stage);
    event ProcessOutgoingEffectHandler OnProcessOutgoingEffect;
}
