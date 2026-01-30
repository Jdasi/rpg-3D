
public abstract class BoardTask
{
    protected bool IsFinished;

    private bool _hasStarted;

    public bool Run()
    {
        if (IsFinished)
        {
            return false;
        }

        if (!_hasStarted)
        {
            _hasStarted = true;
            OnStart();
        }

        OnUpdate();

        if (IsFinished)
        {
            OnCleanup();
        }

        return !IsFinished;
    }

    protected virtual void OnStart() { }
    protected virtual void OnCleanup() { }
    protected virtual void OnUpdate() { }
}
