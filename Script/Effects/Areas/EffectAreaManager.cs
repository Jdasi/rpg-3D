using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectAreaManager : MonoBehaviour
{
    [Serializable]
    public struct PrefabDefinition
    {
        public SkillIds Key;
        public EffectArea Prefab;
    }

    private static EffectAreaManager _instance;

    [SerializeField] List<PrefabDefinition> _prefabs;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }

        _instance = this;
    }

    private void OnDestroy()
    {
        if (_instance != this)
        {
            return;
        }
    }

    public static EffectArea Spawn(SkillIds key)
    {
        Debug.Assert(_instance != null, "[EffectAreaManager] Create - instance was null");

        int index = _instance._prefabs.FindIndex(elem => elem.Key == key);

        if (index < 0)
        {
            Debug.LogError($"[EffectAreaManager] Create - failed lookup for key: {key}");
            return null;
        }

        // TODO - add to list for management..

        return Instantiate(_instance._prefabs[index].Prefab);
    }
}
