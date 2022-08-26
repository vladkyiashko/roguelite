using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/GameEvent/IntGameEvent")]
public class IntGameEvent : ScriptableObject
{
    private readonly List<IntGameEventListener> Listeners = new();

    public void Raise(int value)
    {
        for (int i = Listeners.Count - 1; i >= 0; i--)
        {
            Listeners[i].OnEventRaised(value);
        }
    }

    public void RegisterListener(IntGameEventListener listener)
    {
        Listeners.Add(listener);
    }

    public void UnregisterListener(IntGameEventListener listener)
    {
        _ = Listeners.Remove(listener);
    }
}
