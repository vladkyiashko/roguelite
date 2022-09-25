using UnityEngine;
using UnityEngine.Events;

public class GenericGameEventListener<T, TEvent> : MonoBehaviour, IGameEventListener<T>
    where TEvent : IGameEvent<T>
{
    [SerializeField] private TEvent Event;
    [SerializeField] private UnityEvent<T> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public virtual void OnEventRaised(T value)
    {
        Response.Invoke(value);
    }
}
