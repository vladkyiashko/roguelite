using UnityEngine;
using UnityEngine.Events;

public class MobHolderGameEventListener : MonoBehaviour
{
    [SerializeField] private MobHolderGameEvent Event;
    [SerializeField] private UnityEvent<MobHolder> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(MobHolder value)
    {
        Response.Invoke(value);
    }
}
