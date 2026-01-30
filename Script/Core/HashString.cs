using System;
using System.Collections.Generic;
using UnityEngine;

public readonly struct HashString : IEquatable<HashString>
{
#if DEBUG
    public string String { get; }
    private static readonly Dictionary<int, string> _lookup = new();
#endif
    public readonly int Hash;

    public HashString(string str)
    {
#if DEBUG
        String = str;
#endif
        Hash = default;
        Hash = StringToHash(str);
    }

    public HashString(int hash)
    {
#if DEBUG
        String = _lookup.TryGetValue(hash, out var str) ? str : hash.ToString();
#endif
        Hash = hash;
    }

    public static implicit operator HashString(string str) => new HashString(str);
    public static implicit operator HashString(int hash) => new HashString(hash);
    public static implicit operator int(HashString hashString) => hashString.Hash;

    public override int GetHashCode() => Hash;

    public override bool Equals(object obj) =>
        obj is HashString other && Equals(other);

    public bool Equals(HashString other) => Hash == other.Hash;

    private int StringToHash(string str)
    {
        const int fnvPrime = 16777619;
        const int offsetBasis = unchecked((int)2166136261);

        int hash = offsetBasis;
        for (int i = 0; i < str.Length; ++i)
        {
            char c = str[i];
            hash ^= c;
            hash *= fnvPrime;
        }
#if DEBUG
        if (_lookup.TryGetValue(hash, out var existing) && existing != str)
        {
            Debug.LogError($"Hash collision: '{str}' and '{existing}' share the same hash {hash}");
        }
        else
        {
            _lookup[hash] = str;
        }
#endif
        return hash;
    }
}
