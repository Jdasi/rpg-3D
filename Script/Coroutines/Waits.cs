using System;
using System.Collections;
using UnityEngine;

public static class Wait
{
    public static IEnumerator ForSeconds(float seconds, bool allowCancel = true)
    {
        float expireTime = Time.time + seconds;

        while (Time.time < expireTime)
        {
            yield return null;

            if (allowCancel && Coroutines.IsCurrentCancelled())
            {
                yield break;
            }
        }
    }

    public static IEnumerator ForSecondsRealtime(float seconds, bool allowCancel = true)
    {
        float expireTime = Time.unscaledTime + seconds;

        while (Time.unscaledTime < expireTime)
        {
            yield return null;

            if (allowCancel && Coroutines.IsCurrentCancelled())
            {
                yield break;
            }
        }
    }

    public static IEnumerator ForHandle(ICoroutineHandle handle, bool allowCancel = true)
    {
        while (!handle.IsFinished)
        {
            yield return null;

            if (allowCancel && Coroutines.IsCurrentCancelled())
            {
                yield break;
            }
        }
    }

    public static IEnumerator Until(Func<bool> predicate, bool allowCancel = true)
    {
        while (!predicate())
        {
            yield return null;

            if (allowCancel && Coroutines.IsCurrentCancelled())
            {
                yield break;
            }
        }
    }
}
