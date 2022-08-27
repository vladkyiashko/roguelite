using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/GameEvent/MobHolderGameEvent")]
public class MobHolderGameEvent : ScriptableObject
{
    private readonly List<MobHolderGameEventListener> Listeners = new();

    public void Raise(MobHolder value)
    {
        for (int i = Listeners.Count - 1; i >= 0; i--)
        {
            Listeners[i].OnEventRaised(value);
        }
    }

    public void RegisterListener(MobHolderGameEventListener listener)
    {
        Listeners.Add(listener);
    }

    public void UnregisterListener(MobHolderGameEventListener listener)
    {
        _ = Listeners.Remove(listener);
    }
}
