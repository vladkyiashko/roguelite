using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class AbstractHealth : MonoBehaviour
{
    [SerializeField] protected float MaxHealth;
    [SerializeField] protected float CurrentHealth;
    [SerializeField] protected UnityEvent OnZeroHealth;
    [SerializeField] protected float DeathAnimDuration;
    private WaitForSeconds DeathAnimWaitForSeconds;
    public event Action<GameObject> OnDeathAnimComplete;
    public float GetCurrentHealth => CurrentHealth;

    protected virtual void Awake()
    {
        DeathAnimWaitForSeconds = new WaitForSeconds(DeathAnimDuration);
    }

    protected virtual void OnEnable()
    {
        CurrentHealth = MaxHealth;
    }

    public virtual void Damage(float damage)
    {
        CommonDamage(damage);
    }

    public virtual void DamageOverTime(float damage)
    {
        CommonDamage(damage*Time.deltaTime);
    }

    protected virtual void CommonDamage(float damage)
    {
        if (CurrentHealth <= 0)
        {
            return;
        }

        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

        if (CurrentHealth == 0)
        {
            OnZeroHealth.Invoke();
            _ = StartCoroutine(DeathAnim());
        }
    }

    private IEnumerator DeathAnim()
    {
        yield return DeathAnimWaitForSeconds;

        OnDeathAnimComplete?.Invoke(gameObject);
    }
}
