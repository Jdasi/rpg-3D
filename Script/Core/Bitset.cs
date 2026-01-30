using System;

public struct Bitset
{
    private const int ALL_BITS_SET = int.MaxValue;

    private int _bits;

    public readonly bool IsAnySet()
    {
        return _bits != 0;
    }

    public readonly bool AreAllSet()
    {
        return _bits == ALL_BITS_SET;
    }

    public readonly bool IsFlagSet(int flag)
    {
        return Bit.IsFlagSet(_bits, flag);
    }

    public void SetFlag(int flag, bool set)
    {
        if (set)
        {
            Bit.SetFlag(ref _bits, flag);
        }
        else
        {
            Bit.ClearFlag(ref _bits, flag);
        }
    }

    public readonly bool IsIndexSet(int index)
    {
        return Bit.IsIndexSet(_bits, index);
    }

    public void SetIndex(int index, bool set)
    {
        if (set)
        {
            Bit.SetIndex(ref _bits, index);
        }
        else
        {
            Bit.ClearIndex(ref _bits, index);
        }
    }

    public void Reset()
    {
        _bits = 0;
    }

    public void SetAll()
    {
        _bits = ALL_BITS_SET;
    }
}

public struct Bitset<T> where T : Enum
{
    private Bitset _bits;

    public readonly bool IsAnySet()
    {
        return _bits.IsAnySet();
    }

    public readonly bool AreAllSet()
    {
        return _bits.AreAllSet();
    }

    public readonly bool IsSet(T flag)
    {
        return _bits.IsFlagSet(Convert.ToInt32(flag));
    }

    public void Set(T flag, bool set)
    {
        _bits.SetFlag(Convert.ToInt32(flag), set);
    }

    public void Reset()
    {
        _bits.Reset();
    }

    public void SetAll()
    {
        _bits.SetAll();
    }
}
