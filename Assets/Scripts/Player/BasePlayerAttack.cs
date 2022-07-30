using System;
using UnityEngine;

public class BasePlayerAttack : MonoBehaviour
{
    public event Action<BasePlayerAttack, Collider2D> OnTriggerEnter;
    [SerializeField] private Transform Transform;
    [SerializeField] private float Delay;
    [SerializeField] private float Damage;
    [SerializeField] private float PushForce;
    [SerializeField] private float StunDuration;
    public WaitForSeconds StunWaitForSeconds { get; private set; }
    public Transform GetTransform => Transform;
    public float GetDelay => Delay;
    public float GetDamage => Damage;
    public float GetPushForce => PushForce;

    private void Awake()
    {
        if (StunDuration > 0)
        {
            StunWaitForSeconds = new(StunDuration);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerEnter?.Invoke(this, other);
    }
}
