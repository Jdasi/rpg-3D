
public class InventoryManager
{
    public Weapon Weapon { get; private set; }

    private readonly CharacterData _owner;

    public InventoryManager(CharacterData owner)
    {
        _owner = owner;
    }

    public void EquipWeapon(WeaponIds weaponId)
    {
        if (Weapon != null)
        {
            UnequipWeapon();
        }

        HandleWeaponEquip(WeaponDatabase.Get(weaponId));
    }

    public void UnequipWeapon()
    {
        HandleWeaponEquip(null);
    }

    public void HandleCombatStart()
    {
    }

    public void HandleCombatEnd()
    {
    }

    public void ProcessSkillCostConfig(SkillIds skillId, SkillCostConfig config)
    {
    }

    public void ProcessTargetingConfig(SkillIds skillId, ITargetingConfig config)
    {
    }

    public void ProcessIncomingEffect(EffectResolveContext context, EffectResolveContext.ProcessStages stage)
    {
    }

    public void ProcessOutgoingEffect(EffectResolveContext context, EffectResolveContext.ProcessStages stage)
    {
        Weapon?.ProcessOutgoingEffect(context, stage);
    }

    private void HandleWeaponEquip(Weapon weapon)
    {
        if (Weapon == weapon)
        {
            return;
        }

        bool equip = weapon != null;

        ApplyWeaponSkills(equip);
        ApplyWeaponEnchant(equip);

        Weapon = weapon;

        return;

        void ApplyWeaponSkills(bool apply)
        {
            if (weapon?.Skills == null)
            {
                return;
            }

            SkillIds[] skills = weapon.Skills(new Weapon.SkillsGetterContext
            {
                User = _owner,
                Quality = weapon.Quality,
            });

            for (int i = 0; i < skills.Length; ++i)
            {
                if (apply)
                {
                    _owner.Skills.Add(skills[i], SkillCategories.Weapon);
                }
                else
                {
                    _owner.Skills.Remove(skills[i], SkillCategories.Weapon);
                }
            }
        }

        void ApplyWeaponEnchant(bool apply)
        {
            if (weapon?.Enchantment == WeaponEnchantmentIds.Invalid)
            {
                return;
            }

            WeaponEnchantments.ProcessApplied(weapon.Enchantment, _owner, apply);
        }
    }
}
