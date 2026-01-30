
public sealed partial class CharacterData
{
    private static class Talents
    {
        public static void SetRank(TalentIds talentId, int rank, CharacterData character)
        {
            switch (talentId)
            {
                case TalentIds.Skill_SolarRay: ProcessSkill(SkillIds.SolarRay); break;
                case TalentIds.Passive_HolyPower: ProcessCondition(ConditionIds.HolyPower); break;
            }

            return;

            void ProcessSkill(SkillIds skillId)
            {
                if (rank > 0)
                {
                    character.Skills.Add(skillId, SkillCategories.Career);
                }
                else
                {
                    character.Skills.Remove(skillId, SkillCategories.Career);
                }
            }

            void ProcessCondition(ConditionIds conditionId)
            {
                if (rank > 0)
                {
                    character._conditionManager.Apply(conditionId, ConditionManager.PERMANENT_DURATION, rank);
                }
                else
                {
                    character._conditionManager.Remove(conditionId, ConditionManager.ALL_STACKS);
                }
            }
        }
    }
}
