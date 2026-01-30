using System.Collections;
using UnityEngine;

public class TokenModel : MonoBehaviour
{
    public float CollisionRadius => _navRadiusType.SizeEnumToCollisionRadius();
    public float NavRadius => _navRadiusType.SizeEnumToNavRadius();
    public float MoveSpeed => _moveSpeedType.SpeedEnumToMoveSpeed();

    [SerializeField] TokenSizes _navRadiusType;
    [SerializeField] TokenSpeeds _moveSpeedType;

    public void Init(CharacterData data)
    {
        OnInit(data);
    }

    public bool IsIdle()
    {
        return true;
    }

    public IEnumerator PlayAction(TokenPlayActionConfig config)
    {
        bool receivedCast = false;

        if (config.TargetPosition.HasValue)
        {
            transform.LookAt(config.TargetPosition.Value);
        }

        // TODO - fire action events

        if (!receivedCast)
        {
            config.OnActionEvent?.Invoke(TokenActionEvents.Cast);
        }

        yield break;
    }

    protected virtual void OnInit(CharacterData data) { }
}
