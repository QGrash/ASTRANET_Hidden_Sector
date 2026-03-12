// Core/DI.cs
using System;
using System.Collections.Generic;

namespace ASTRANET.Core;

public static class DI
{
    private static readonly Dictionary<Type, object> _instances = new();

    public static void Register<T>(T instance) where T : class
    {
        _instances[typeof(T)] = instance;
    }

    public static T Resolve<T>() where T : class
    {
        if (_instances.TryGetValue(typeof(T), out var instance))
            return instance as T;
        throw new InvalidOperationException($"Type {typeof(T)} not registered.");
    }
}