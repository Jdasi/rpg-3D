using System.Collections.Generic;
using UnityEngine;

// can't do classless comparers as this C# version doesn't have:
// public static void Sort<T>(T[] array, int index, int length, Comparison<T> comparer);
public static class JComparer
{
    public static readonly RaycastHitDistanceComparer RaycastHitClosest = new RaycastHitDistanceComparer();

    public class RaycastHitDistanceComparer : IComparer<RaycastHit>
    {
        public int Compare(RaycastHit a, RaycastHit b)
        {
            return a.distance.CompareTo(b.distance);
        }
    }
}
