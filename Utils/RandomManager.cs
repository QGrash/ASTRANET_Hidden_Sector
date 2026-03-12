// Utils/RandomManager.cs
using System;

namespace ASTRANET.Utils;

public static class RandomManager
{
    private static Random _random = new Random();

    public static void SetSeed(int seed)
    {
        _random = new Random(seed);
    }

    public static int Next() => _random.Next();
    public static int Next(int maxValue) => _random.Next(maxValue);
    public static int Next(int minValue, int maxValue) => _random.Next(minValue, maxValue);
    public static double NextDouble() => _random.NextDouble();
}