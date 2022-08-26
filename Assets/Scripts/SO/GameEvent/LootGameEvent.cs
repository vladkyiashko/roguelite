using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/GameEvent/LootGameEvent")]
public class LootGameEvent : ScriptableObject
{
    private readonly List<LootGameEventListener> Listeners = new();

    public void Raise(Loot value)
    {
        for (int i = Listeners.Count - 1; i >= 0; i--)
        {
            Listeners[i].OnEventRaised(value);
        }
    }

    public void RegisterListener(LootGameEventListener listener)
    {
        Listeners.Add(listener);
    }

    public void UnregisterListener(LootGameEventListener listener)
    {
        _ = Listeners.Remove(listener);
    }
}
