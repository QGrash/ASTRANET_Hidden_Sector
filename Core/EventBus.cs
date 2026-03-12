// Core/EventBus.cs
using System;
using System.Collections.Generic;

namespace ASTRANET.Core;

public static class EventBus
{
    private static readonly Dictionary<Type, List<Action<GameEvent>>> _subscribers = new();

    public static void Subscribe<T>(Action<T> handler) where T : GameEvent
    {
        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new List<Action<GameEvent>>();

        _subscribers[type].Add(e => handler((T)e));
    }

    public static void Publish<T>(T eventData) where T : GameEvent
    {
        var type = typeof(T);
        if (_subscribers.ContainsKey(type))
        {
            foreach (var handler in _subscribers[type])
            {
                handler(eventData);
            }
        }
    }
}