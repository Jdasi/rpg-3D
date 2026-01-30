using static EffectTypes;

public static class EffectCollectionHelpers
{
    public static void MultiplyElement(this EffectCollection col, ElementTypes type, float factor)
    {
        foreach (var effect in col)
        {
            if (effect is not DealElement element)
            {
                continue;
            }

            if (element.Type != type)
            {
                continue;
            }

            element.BaseValue = (int)(element.BaseValue * factor);
            element.FinalValue = (int)(element.FinalValue * factor);
        }
    }

    public static void MultiplyElements(this EffectCollection col, float factor)
    {
        foreach (var effect in col)
        {
            if (effect is not DealElement element)
            {
                continue;
            }

            element.BaseValue = (int)(element.BaseValue * factor);
            element.FinalValue = (int)(element.FinalValue * factor);
        }
    }

    public static void MultiplyElements(this EffectCollection col, ElementTypes elements, float factor)
    {
        foreach (var effect in col)
        {
            if (effect is not DealElement element)
            {
                continue;
            }

            if (!elements.HasFlag(element.Type))
            {
                continue;
            }

            element.BaseValue = (int)(element.BaseValue * factor);
            element.FinalValue = (int)(element.FinalValue * factor);
        }
    }

    public static void AddWeaponScaledElement(this EffectCollection col, CharacterData user, ElementTypes element, int baseValue,
        StatTypes stat1 = StatTypes.Invalid, StatTypes stat2 = StatTypes.Invalid, StatTypes stat3 = StatTypes.Invalid)
    {
        AddScaledElement(col, user, element, baseValue, DealElement.BASE_VARIANCE, stat1, stat2, stat3);
    }

    public static void AddScaledElement(this EffectCollection col, CharacterData user, ElementTypes element, int baseValue, float variance,
        StatTypes stat1 = StatTypes.Invalid, StatTypes stat2 = StatTypes.Invalid, StatTypes stat3 = StatTypes.Invalid)
    {
        baseValue = user.Inventory.Weapon.CalculateScaling(baseValue);
        int scaledValue = 0;

        if (stat1 == StatTypes.Invalid)
        {
            scaledValue += user.Inventory.Weapon.CalculateScaling(user, stat1);
        }

        if (stat2 != StatTypes.Invalid)
        {
            scaledValue += user.Inventory.Weapon.CalculateScaling(user, stat2);
        }

        if (stat3 != StatTypes.Invalid)
        {
            scaledValue += user.Inventory.Weapon.CalculateScaling(user, stat3);
        }

        col.Add(new DealElement(element, baseValue + scaledValue, variance));
    }
}
