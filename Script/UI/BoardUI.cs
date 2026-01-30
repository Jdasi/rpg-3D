using UnityEngine;

public class BoardUI : MonoBehaviour
{
    public static BoardUI Instance { get; private set; }

    [SerializeField] SkillButton[] _skillButtons;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        foreach (var button in _skillButtons)
        {
            button.gameObject.SetActive(false);
        }

        LocalEvents.TurnStarted.Subscribe(OnTurnStarted);
    }

    private void OnDestroy()
    {
        if (Instance != this)
        {
            return;
        }

        Instance = null;

        LocalEvents.TurnStarted.Unsubscribe(OnTurnStarted);
    }

    private void OnTurnStarted(Token token)
    {
        var skills = token.Data.Skills.GetSkills();

        for (int i = 0; i < _skillButtons.Length; ++i)
        {
            Skill skill;

            if (i == 0)
            {
                skill = SkillDatabase.Get(SkillIds.Attack);
            }
            else if (skills.MoveNext())
            {
                skill = skills.Current;
            }
            else
            {
                skill = null;
            }

            if (skill != null)
            {
                _skillButtons[i].Init(skill.Id);
            }

            _skillButtons[i].gameObject.SetActive(skill != null);
        }
    }
}
