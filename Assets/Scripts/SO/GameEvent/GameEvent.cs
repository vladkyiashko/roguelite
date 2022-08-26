using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/GameEvent/GameEvent")]
public class GameEvent : ScriptableObject
{
    private readonly List<GameEventListener> Listeners = new();

    public void Raise()
    {
        for (int i = Listeners.Count - 1; i >= 0; i--)
        {
            Listeners[i].OnEventRaised();
        }
    }

    public void RegisterListener(GameEventListener listener)
    {
        Listeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener)
    {
        _ = Listeners.Remove(listener);
    }
}
