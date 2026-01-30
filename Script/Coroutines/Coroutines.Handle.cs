using System;

public interface ICoroutineHandle
{
    bool IsFinished { get; }
    bool IsCancelled { get; }
    void Cancel(Coroutines.CancelModes mode = Coroutines.CancelModes.Deferred);
}

public partial class Coroutines
{
    public enum CancelModes
    {
        None,

        /// <summary>Processing will continue. Routines can handle cancellation via <see cref="IsCurrentCancelled"/>.</summary>
        Deferred,

        /// <summary>Processing will stop abruptly. Note: Consider using additional cancellation logic when using this.</summary>
        Immediate,
    }

    private class Handle : ICoroutineHandle
    {
        public bool IsFinished => _remaining == 0;
        public bool IsCancelled => CancelMode != CancelModes.None;
        public CancelModes CancelMode { get; private set; }

        private int _remaining;

        public Handle(int groupCount)
        {
            _remaining = groupCount;
        }

        public void Cancel(CancelModes type)
        {
            if (type > CancelMode)
            {
                CancelMode = type;
            }
        }

        public void OnChildrenAdded(int amount)
        {
            _remaining += amount;
        }

        public void OnChildComplete()
        {
            _remaining = Math.Max(0, _remaining - 1);
        }
    }
}
