using UnityEngine;

public abstract class AbstractHealth : MonoBehaviour
{
    [SerializeField] protected float MaxHealth;
    [SerializeField] protected float CurrentHealth;

    private void OnEnable()
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

        if (CurrentHealth <= 0)
        {
            Debug.LogError(gameObject + " zero health");
        }
    }
}
