using System.Collections.Generic;
using UnityEngine;

public class GenericGameEvent<T> : ScriptableObject, IGameEvent<T>
{
    private readonly List<IGameEventListener<T>> Listeners = new();

    public void Raise(T value)
    {
        for (int i = Listeners.Count - 1; i >= 0; i--)
        {
            Listeners[i].OnEventRaised(value);
        }
    }

    public void RegisterListener(IGameEventListener<T> listener)
    {
        Listeners.Add(listener);
    }

    public void UnregisterListener(IGameEventListener<T> listener)
    {
        _ = Listeners.Remove(listener);
    }
}
