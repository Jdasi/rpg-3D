using UnityEngine;

public class TokenPlayActionConfig
{
    public TokenActions Action { get; init; }
    public Vector3? TargetPosition { get; init; }

    public delegate void ActionEventHandler(TokenActionEvents @event);
    public ActionEventHandler OnActionEvent { get; init; }
}
