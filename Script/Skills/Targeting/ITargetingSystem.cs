using UnityEngine;

public interface ITargetingConfig
{
    TargetingFlags Flags { get; set; }
}

public interface ITargetingSystem
{
    SkillContext GetContext();
    void Init(SkillContext context, ITargetingConfig config);
    void Cleanup();
    bool CanResolve();
    void SetVisible(bool visible);
    void PlayerUpdate(LayerMask mouseLayer, bool force = false);
}
