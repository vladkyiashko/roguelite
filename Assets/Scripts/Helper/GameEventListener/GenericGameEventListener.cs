using UnityEngine;
using UnityEngine.Events;

public class GenericGameEventListener<T, TEvent> : MonoBehaviour, IGameEventListener<T>
    where TEvent : IGameEvent<T>
    //where TEvent : GenericGameEvent<T, GenericGameEventListener<T, TEvent>>
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

    public void OnEventRaised(T value)
    {
        Response.Invoke(value);
    }
}
