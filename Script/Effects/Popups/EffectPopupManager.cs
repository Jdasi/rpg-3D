using UnityEngine;

public class EffectPopupManager : MonoBehaviour
{
    [SerializeField] EffectPopup _prefab;

    private void Start()
    {
        LocalEvents.CharacterHealthEffect.Subscribe(OnCharacterHealthEffect);
    }

    private void OnDestroy()
    {
        LocalEvents.CharacterHealthEffect.Unsubscribe(OnCharacterHealthEffect);
    }

    private void CreatePopup(CharacterId characterId, string text, Color color)
    {
        if (!BoardControl.Instance.TryGetToken(characterId, out var token))
        {
            return;
        }

        var clone = Instantiate(_prefab);
        clone.transform.position = token.transform.position + (Vector3.up * 0.25f) + (Vector3.forward * 0.15f);
        clone.Init(text, color);
    }

    private void OnCharacterHealthEffect(LocalEventData.CharacterHealthEffect data)
    {
        string text;

        if (data.Amount > 0)
        {
            text = $"{(data.IsDamage ? "-" : "+")}{data.Amount}";
        }
        else
        {
            text = data.Amount.ToString();
        }

        if (data.IsDamage)
        {
            if (data.Flags.IsSet(EffectTempFlags.Absorbed))
            {
                text += $" ({data.AbsorbAmount} Absorbed)";
            }
            else if (data.Flags.IsSet(EffectTempFlags.Hit))
            {
                if (data.Flags.IsSet(EffectTempFlags.Crit))
                {
                    text += "!";
                }

                if (data.Flags.IsSet(EffectTempFlags.Blocked))
                {
                    text += " (Blocked)";
                }
            }
            else
            {
                text += " (Grazed)";
            }
        }

        CreatePopup(data.CharacterId, text, data.IsDamage ? Color.red : Color.green);
    }
}
