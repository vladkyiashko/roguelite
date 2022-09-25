using UnityEngine;
using UnityEngine.Events;

public class BoolGameEventListener : GenericGameEventListener<bool, BoolGameEvent>
{
    [SerializeField] private UnityEvent<bool> InvertedResponse;

    public override void OnEventRaised(bool value)
    {
        base.OnEventRaised(value);
        InvertedResponse.Invoke(!value);
    }
}
