using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/GameEvent/FloatGameEvent")]
public class FloatGameEvent : ScriptableObject
{
    private readonly List<FloatGameEventListener> Listeners = new();

    public void Raise(float value)
    {
        for (int i = Listeners.Count - 1; i >= 0; i--)
        {
            Listeners[i].OnEventRaised(value);
        }
    }

    public void RegisterListener(FloatGameEventListener listener)
    {
        Listeners.Add(listener);
    }

    public void UnregisterListener(FloatGameEventListener listener)
    {
        _ = Listeners.Remove(listener);
    }
}
