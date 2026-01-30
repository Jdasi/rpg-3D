using UnityEngine.AI;

public class BoardTask_StartTurn : BoardTask
{
    private readonly Token _token;

    public BoardTask_StartTurn(Token token)
    {
        _token = token;
    }

    protected override void OnStart()
    {
        _token.SetObstacleEnabled(false);
    }

    protected override void OnUpdate()
    {
        if (!NavMesh.SamplePosition(_token.transform.position, out _, 0.1f, NavMesh.AllAreas))
        {
            return;
        }

        BoardControl.Instance.Walker.Carry(_token);
        // TODO - refresh character resources

        LocalEvents.TurnStarted.Invoke(_token);
        IsFinished = true;
    }
}
