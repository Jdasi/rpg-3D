using System.Collections;
using System.Collections.Generic;

public partial class Coroutines
{
    private readonly struct Wrapper
    {
        public readonly Handle Handle;

        private readonly Stack<IEnumerator> _stack;

        public Wrapper(Handle handle, IEnumerator routine)
        {
            Handle = handle;
            _stack = new Stack<IEnumerator>(2);
            _stack.Push(routine);
        }

        public bool Run()
        {
            if (IsAborting())
            {
                return false;
            }

            if (EnableShallowMode)
            {
                return RunShallow();
            }

            return RunDeep();
        }

        private bool IsAborting()
        {
            return Handle.CancelMode == CancelModes.Immediate;
        }

        /// <summary>
        /// Run the current enumerator and wait a frame regardless of the result.
        /// </summary>
        private bool RunShallow()
        {
            IEnumerator enumerator = _stack.Peek();

            if (!enumerator.MoveNext())
            {
                if (IsAborting())
                {
                    return false;
                }

                _stack.Pop();
                return _stack.Count > 0;
            }

            if (IsAborting())
            {
                return false;
            }

            if (enumerator.Current is IEnumerator nested)
            {
                _stack.Push(nested);
            }

            return true;
        }

        /// <summary>
        /// Keep running the stack until an enumerator wants to wait a frame.
        /// </summary>
        private bool RunDeep()
        {
            IEnumerator enumerator = _stack.Peek();

            while (enumerator != null)
            {
                if (!enumerator.MoveNext())
                {
                    if (IsAborting())
                    {
                        return false;
                    }

                    if (_stack.Count == 1)
                    {
                        return false;
                    }

                    _stack.Pop();
                    enumerator = _stack.Peek();

                    continue;
                }

                switch (enumerator.Current)
                {
                    // wait one frame
                    case null: return true;

                    case IEnumerator nested:
                    {
                        _stack.Push(nested);
                        enumerator = nested;
                    } break;
                }
            }

            return false;
        }
    }
}
