using UnityEngine;
using UnityEngine.Events;

public class LootGameEventListener : MonoBehaviour
{
    [SerializeField] private LootGameEvent Event;
    [SerializeField] private UnityEvent<Loot> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(Loot value)
    {
        Response.Invoke(value);
    }
}
