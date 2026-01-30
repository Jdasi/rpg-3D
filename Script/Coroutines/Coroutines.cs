using System.Collections;
using System.Collections.Generic;

public partial class Coroutines
{
    /// <summary>
    /// When set to TRUE, each coroutine is run once per frame.
    /// Otherwise, each coroutine is run until it requests to wait a frame.
    /// </summary>
    public static bool EnableShallowMode = false;

    private static readonly List _coroutines = new List(8);
    private static int? _it;

    public static ICoroutineHandle Start(IEnumerator routine, bool inheritCurrentHandle = false)
    {
        Handle handle = GetValidHandle(inheritCurrentHandle, 1);
        _coroutines.Add(new Wrapper(handle, routine));
        return handle;
    }

    public static ICoroutineHandle Start(ICollection<IEnumerator> routines, bool inheritCurrentHandle = false)
    {
        Handle handle = GetValidHandle(inheritCurrentHandle, routines.Count);

        foreach (IEnumerator routine in routines)
        {
            _coroutines.Add(new Wrapper(handle, routine));
        }

        return handle;
    }

    public static bool IsCurrentCancelled()
    {
        return _it.HasValue && _coroutines[_it.Value].Handle.IsCancelled;
    }

    public static void CancelCurrent(CancelModes mode = CancelModes.Deferred)
    {
        if (_it.HasValue)
        {
            _coroutines[_it.Value].Handle.Cancel(mode);
        }
    }

    public static void CancelAll(CancelModes mode = CancelModes.Deferred)
    {
        for (int i = 0; i < _coroutines.Count; ++i)
        {
            _coroutines[i].Handle.Cancel(mode);
        }
    }

    public void Update()
    {
        if (_coroutines.Count == 0)
        {
            return;
        }

        for (int i = _coroutines.Count - 1; i >= 0; --i)
        {
            _it = i;

            if (_coroutines[i].Run())
            {
                continue;
            }

            _coroutines[i].Handle.OnChildComplete();
            _coroutines.RemoveAt(i);
        }

        _it = null;
    }

    private static Handle GetValidHandle(bool inheritCurrent, int groupCount)
    {
        Handle handle;

        if (inheritCurrent && _it.HasValue)
        {
            handle = _coroutines[_it.Value].Handle;
            handle.OnChildrenAdded(groupCount);
        }
        else
        {
            handle = new Handle(groupCount);
        }

        return handle;
    }
}
