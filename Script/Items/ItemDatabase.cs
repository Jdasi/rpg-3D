using System;
using UnityEngine;

[Serializable]
public struct DatabaseWeapon
{
    public WeaponIds Id;
    public GameObject Prefab;
}

public class ItemDatabase : MonoBehaviour
{
    [SerializeField] DatabaseWeapon[] _prefabWeapons;

    private static ItemDatabase _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
    }

    public static GameObject SpawnWeapon(WeaponIds id, Transform parent = null)
    {
        for (int i = 0; i < _instance._prefabWeapons.Length; ++i)
        {
            if (_instance._prefabWeapons[i].Id != id)
            {
                continue;
            }

            return Instantiate(_instance._prefabWeapons[i].Prefab, parent);
        }

        return null;
    }
}
