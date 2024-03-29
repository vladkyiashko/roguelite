using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class AbstractHealth : MonoBehaviour
{
    [SerializeField] protected BaseUnitBalance Balance;
    [SerializeField] protected UnityEvent OnZeroHealth;
    [SerializeField] protected TransformGameEvent OnDeathAnimComplete;
    private WaitForSeconds DeathAnimWaitForSeconds;
    protected float CurrentHealth;
    public float GetCurrentHealth => CurrentHealth;

    protected virtual void Awake()
    {
        DeathAnimWaitForSeconds = new WaitForSeconds(Balance.DeathAnimDuration);
    }

    protected virtual void OnEnable()
    {
        CurrentHealth = Balance.MaxHealth;
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
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, Balance.MaxHealth);

        if (CurrentHealth == 0)
        {
            OnZeroHealth.Invoke();
            _ = StartCoroutine(DeathAnim());
        }
    }

    protected virtual IEnumerator DeathAnim()
    {
        yield return DeathAnimWaitForSeconds;

        OnDeathAnimComplete.Raise(transform);
    }
}
