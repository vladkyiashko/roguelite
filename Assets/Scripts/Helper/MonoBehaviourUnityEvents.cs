using UnityEngine;
using UnityEngine.Events;

public class MonoBehaviourUnityEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent AwakeEvent;
    [SerializeField] private UnityEvent OnEnableEvent;
    [SerializeField] private UnityEvent StartEvent;
    [SerializeField] private UnityEvent OnDisableEvent;
    [SerializeField] private UnityEvent OnDestroyEvent;
    [SerializeField] private UnityEvent OnTriggerEnterEvent;
    [SerializeField] private UnityEvent OnTriggerExitEvent;
    [SerializeField] private UnityEvent OnCollisionEnterEvent;
    [SerializeField] private UnityEvent OnCollisionExitEvent;

    private void Awake()
    {
        AwakeEvent.Invoke();
    }

    private void OnEnable()
    {
        OnEnableEvent.Invoke();
    }

    private void Start()
    {
        StartEvent.Invoke();
    }

    private void OnDisable()
    {
        OnDisableEvent.Invoke();
    }

    private void OnDestroy()
    {
        OnDestroyEvent.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerEnterEvent.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnTriggerExitEvent.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        OnCollisionEnterEvent.Invoke();
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        OnCollisionExitEvent.Invoke();
    }
}
