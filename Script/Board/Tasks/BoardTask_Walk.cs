using System.Collections;
using UnityEngine;

public class BoardTask_Walk : BoardTask
{
    private readonly Vector3 _destination;
    private readonly float _radius;

    private ICoroutineHandle _routine;

    public BoardTask_Walk(Vector3 destination)
    {
        _destination = destination;
    }

    protected override void OnStart()
    {
        _routine = Coroutines.Start(Wrap());

        return;

        IEnumerator Wrap()
        {
            yield return BoardControl.Instance.Walker.WalkSequence(_destination, _radius);
            IsFinished = true;
        }
    }

    protected override void OnCleanup()
    {
        _routine.Cancel();
    }
}
