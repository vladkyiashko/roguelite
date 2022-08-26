using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/GameEvent/TransformGameEvent")]
public class TransformGameEvent : ScriptableObject
{
    private readonly List<TransformGameEventListener> Listeners = new();

    public void Raise(Transform value)
    {
        for (int i = Listeners.Count - 1; i >= 0; i--)
        {
            Listeners[i].OnEventRaised(value);
        }
    }

    public void RegisterListener(TransformGameEventListener listener)
    {
        Listeners.Add(listener);
    }

    public void UnregisterListener(TransformGameEventListener listener)
    {
        _ = Listeners.Remove(listener);
    }
}
