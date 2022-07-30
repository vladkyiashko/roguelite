using System.Collections;
using UnityEngine;

public class AutoPoolDestroy : MonoBehaviour
{
    [SerializeField] private float Delay;
    private WaitForSeconds DelayWaitForSeconds;

    private void Awake()
    {
        DelayWaitForSeconds ??= new WaitForSeconds(Delay);
    }

    private void OnEnable()
    {
        _ = StartCoroutine(DestroyDelayed());
    }

    private IEnumerator DestroyDelayed()
    {
        yield return DelayWaitForSeconds;
        LocalObjectPool.Destroy(gameObject);
    }
}
