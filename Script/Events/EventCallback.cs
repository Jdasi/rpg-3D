
public struct EventCallback
{
    public delegate void CallbackHandler();
    private CallbackHandler _callback;

    public void Subscribe(CallbackHandler handler)
    {
        _callback += handler;
    }

    public void Unsubscribe(CallbackHandler handler)
    {
        _callback -= handler;
    }

    public void Invoke()
    {
        _callback?.Invoke();
    }
}

public struct EventCallback<T>
{
    public delegate void CallbackHandler(T data);
    private CallbackHandler _callback;

    public void Subscribe(CallbackHandler handler)
    {
        _callback += handler;
    }

    public void Unsubscribe(CallbackHandler handler)
    {
        _callback -= handler;
    }

    public void Invoke(T data)
    {
        _callback?.Invoke(data);
    }
}
