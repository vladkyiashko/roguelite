using UnityEngine;
using UnityEngine.Events;

public class TransformGameEventListener : MonoBehaviour
{
    [SerializeField] private TransformGameEvent Event;
    [SerializeField] private UnityEvent<Transform> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(Transform value)
    {
        Response.Invoke(value);
    }
}
