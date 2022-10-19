using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnter2DEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent<Collider2D> OnTriggerEnter;
    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerEnter.Invoke(other);
    }
}
