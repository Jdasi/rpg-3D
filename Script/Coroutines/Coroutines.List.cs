using System;

public partial class Coroutines
{
    private class List
    {
        public int Count => _count;

        private Wrapper[] _items;
        private int _count;

        public List(uint capacity = 4)
        {
            _items = new Wrapper[capacity];
            _count = 0;
        }

        public void Add(Wrapper item)
        {
            if (_count == _items.Length)
            {
                Array.Resize(ref _items, _items.Length == 0 ? 4 : _items.Length * 2);
            }

            _items[_count++] = item;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
            {
                throw new IndexOutOfRangeException();
            }

            int lastIndex = _count - 1;

            if (index < lastIndex)
            {
                Array.Copy(_items, index + 1, _items, index, lastIndex - index);
            }

            _items[lastIndex] = default;
            --_count;
        }

        public ref Wrapper this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                {
                    throw new IndexOutOfRangeException();
                }

                return ref _items[index];
            }
        }
    }
}
