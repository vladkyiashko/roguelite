using UnityEngine;
using UnityEngine.Events;

public class MonoBehaviourUnityEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent AwakeEvent;
    [SerializeField] private UnityEvent OnEnableEvent;
    [SerializeField] private UnityEvent StartEvent;
    [SerializeField] private UnityEvent OnDisableEvent;
    [SerializeField] private UnityEvent OnDestroyEvent;

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
}
