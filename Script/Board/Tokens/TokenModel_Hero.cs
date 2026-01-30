using UnityEngine;

public class TokenModel_Hero : TokenModel
{
    [SerializeField] Transform _weaponRoot;

    private CharacterData _data;

    protected override void OnInit(CharacterData data)
    {
        _data = data;
        ItemDatabase.SpawnWeapon(data.Inventory.Weapon.WeaponId, _weaponRoot);
    }
}
