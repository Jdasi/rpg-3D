using System.Collections.Generic;
using UnityEngine;

public partial class TargetingRay : MonoBehaviour
{
    public static List<Token> Test(Vector3 position, Vector3 forward, TargetingTypes.Ray.TargetingConfig config, CharacterData user)
    {
        var ray = Instantiate(BoardControl.Instance.PrefabTargetingRay_NoVis, position, Quaternion.LookRotation(forward));
        ray.Init(config);
        return RayTest(ray, user);
    }

    private static List<Token> RayTest(TargetingRay ray, CharacterData user)
    {
        ray.SetTargetingInfo(user);
        ray.FixedUpdate();

        var collisions = ray._collisions;
        Destroy(ray.gameObject);
        return collisions;
    }
}
