using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = System.Random;

internal static class Rand
{
    private static Random _rand;

    static Rand()
    {
        _rand = new Random();
    }

    /// <summary>
    /// Re-initialize the generator.
    /// </summary>
    /// <param name="seed">If valid, initialize with this seed. Otherwise, generate a new seed and then initialize.</param>
    /// <returns>The new seed.</returns>
    public static int Seed(int? seed = null)
    {
        if (seed == null)
        {
            seed = RollEx(int.MinValue, int.MaxValue);
        }

        _rand = new Random(seed.Value);
        return seed.Value;
    }

    /// <summary>
    /// Rolls between <paramref name="minValue"/> and <paramref name="minValue"/>.
    /// </summary>
    public static int RollEx(int minValue, int maxValue, int repeat = 0)
    {
        int total = 0;

        for (int i = -1; i < repeat; ++i)
        {
            total += _rand.Next(minValue, maxValue);
        }

        return total;
    }

    /// <summary>
    /// Rolls between <paramref name="minValue"/> and <paramref name="maxValue"/> (inclusive).
    /// </summary>
    public static int Roll(int minValue, int maxValue, int repeat = 0)
    {
        return RollEx(minValue, maxValue + 1, repeat);
    }

    /// <summary>
    /// Rolls between a calculated min and max of <paramref name="value"/> based on <paramref name="variance"/> (inclusive).
    /// </summary>
    public static int Roll(int value, float variance, int repeat = 0)
    {
        variance = value * variance;
        return Roll((int)Math.Floor(value - variance), (int)Math.Ceiling(value + variance), repeat);
    }

    /// <summary>
    /// Rolls between 0 and 99, returning true if the value is under <paramref name="target"/>.
    /// </summary>
    public static bool RollUnder(int target)
    {
        return RollEx(0, 100) < target;
    }

    /// <summary>
    /// Generates a random float between min and max.
    /// </summary>
    public static float Float(float min, float max)
    {
        return (float)(_rand.NextDouble() * (max - min) + min);
    }

    /// <summary>
    /// Simulates a coin flip. Roughly a 50% chance of an outcome.
    /// </summary>
    public static bool CoinFlip()
    {
        return Roll(0, 2) == 1;
    }

    /// <summary>
    /// Returns a random element from <paramref name="collection"/>
    /// </summary>
    public static T Select<T>(ICollection<T> collection)
    {
        Debug.Assert(collection != null, $"[Rand] Select<{typeof(T).Name}> - collection was null");
        Debug.Assert(collection.Count > 0, $"[Rand] Select<{typeof(T).Name}> - collection was empty");
        return collection.ElementAt(RollEx(0, collection.Count));
    }

    /// <summary>
    /// Returns a random point within a circle.
    /// </summary>
    public static Vector2 InsideUnitCircle(float scale = 1)
    {
        double angle = _rand.NextDouble() * 2 * Math.PI; // Random angle in radians
        double radius = Math.Sqrt(_rand.NextDouble());   // Random radius within [0, 1]

        // Convert polar coordinates to Cartesian coordinates
        float x = (float)(Math.Cos(angle) * radius);
        float y = (float)(Math.Sin(angle) * radius);

        return new Vector2(x, y) * scale;
    }

    /// <summary>
    /// Returns a random point within a sphere.
    /// </summary>
    public static Vector3 InsideUnitSphere(float scale = 1)
    {
        double u = _rand.NextDouble(); // Random value between 0 and 1
        double v = _rand.NextDouble(); // Random value between 0 and 1
        double theta = 2 * Math.PI * u;
        double phi = Math.Acos(2 * v - 1);

        // Convert spherical coordinates to Cartesian coordinates
        float x = (float)(Math.Sin(phi) * Math.Cos(theta));
        float y = (float)(Math.Sin(phi) * Math.Sin(theta));
        float z = (float)(Math.Cos(phi));

        return new Vector3(x, y, z) * scale;
    }
}
