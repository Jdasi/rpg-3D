using System.Collections.Generic;
using UnityEngine;

public partial class TargetingShape
{
    public static List<Token> TestLine(Vector3 position, Vector3 forward, float width, float length, CharacterData user, TargetingFlags flags)
    {
        var shape = Instantiate(BoardControl.Instance.PrefabTargetingShape_NoVis, position, Quaternion.LookRotation(forward));
        shape.InitLine(width, width, length);
        return ShapeTest(shape, user, flags);
    }

    public static List<Token> TestCone(Vector3 position, Vector3 forward, float width, float length, CharacterData user, TargetingFlags flags)
    {
        var shape = Instantiate(BoardControl.Instance.PrefabTargetingShape_NoVis, position, Quaternion.LookRotation(forward));
        shape.InitTriangle(width, length);
        return ShapeTest(shape, user, flags);
    }

    public static List<Token> TestArc(Vector3 position, Vector3 forward, float radius, float angle, CharacterData user, TargetingFlags flags)
    {
        var shape = Instantiate(BoardControl.Instance.PrefabTargetingShape_NoVis, position, Quaternion.LookRotation(forward));
        shape.InitArc(radius, angle);
        return ShapeTest(shape, user, flags);
    }

    public static List<Token> TestCircle(Vector3 position, float radius, CharacterData user, TargetingFlags flags)
    {
        var shape = Instantiate(BoardControl.Instance.PrefabTargetingShape_NoVis, position, Quaternion.identity);
        shape.InitCircle(radius);
        return ShapeTest(shape, user, flags);
    }

    private static List<Token> ShapeTest(TargetingShape shape, CharacterData user, TargetingFlags flags)
    {
        shape.SetTargetingInfo(user, flags);

        Physics.simulationMode = SimulationMode.Script;
        Physics.Simulate(Time.fixedDeltaTime);
        Physics.simulationMode = SimulationMode.FixedUpdate;

        var collisions = shape._collisions;
        Destroy(shape.gameObject);
        return collisions;
    }
}
