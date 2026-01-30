
public class BoardTask_Skill : BoardTask
{
    private readonly int _seed;
    private readonly Skill _skill;
    private readonly SkillContext _context;

    public BoardTask_Skill(int seed, Skill skill, SkillContext context)
    {
        _seed = seed;
        _skill = skill;
        _context = context;
    }

    protected override void OnStart()
    {
        LocalEvents.SkillFinished.Subscribe(OnSkillFinished);

        Rand.Seed(_seed);
        Coroutines.Start(_skill.Resolve(_context));
    }

    protected override void OnCleanup()
    {
        LocalEvents.SkillFinished.Unsubscribe(OnSkillFinished);
    }

    private void OnSkillFinished(LocalEventData.SkillFinished data)
    {
        IsFinished = true;
    }
}
