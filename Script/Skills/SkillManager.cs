using System.Collections.Generic;

public class SkillManager
{
    private class Data
    {
        public Skill Skill { get; }
        public int RequestCount;

        public Data(SkillIds id)
        {
            Skill = SkillDatabase.Get(id);
        }
    }

    private Dictionary<SkillIds, Data>[] _skillDicts;

    public SkillManager()
    {
        _skillDicts = new Dictionary<SkillIds, Data>[(int)SkillCategories.RESERVED_COUNT];
    }

    public void Add(SkillIds id, SkillCategories category)
    {
        var dict = _skillDicts[(int)category] ??= new Dictionary<SkillIds, Data>();

        if (!dict.TryGetValue(id, out var data))
        {
            data = new Data(id);
            dict[id] = data;
        }

        ++data.RequestCount;
    }

    public void Remove(SkillIds id, SkillCategories category)
    {
        var dict = _skillDicts[(int)category];

        if (dict == null || !dict.TryGetValue(id, out var data))
        {
            return;
        }

        --data.RequestCount;

        if (data.RequestCount > 0)
        {
            return;
        }

        if (dict.Count == 1)
        {
            _skillDicts[(int)category] = null;
        }
        else
        {
            dict.Remove(id);
        }
    }

    public IEnumerator<Skill> GetSkills()
    {
        for (int i = 0; i < (int)SkillCategories.RESERVED_COUNT; ++i)
        {
            var dict = _skillDicts[i];

            if (dict == null)
            {
                yield break;
            }

            foreach (var elem in dict)
            {
                yield return elem.Value.Skill;
            }
        }
    }

    public IEnumerator<Skill> GetSkills(SkillCategories category)
    {
        var dict = _skillDicts[(int)category];

        if (dict == null)
        {
            yield break;
        }

        foreach (var elem in dict)
        {
            yield return elem.Value.Skill;
        }
    }
}
