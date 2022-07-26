using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float MaxHealth;
    [SerializeField] private float CurrentHealth;    
    public event Action<float> OnHealthPercentChanged;

    private void Awake()
    {        
        CurrentHealth = MaxHealth;
    }

    private void Start()
    {
        OnHealthPercentChangedInvoke();
    }

    private void OnHealthPercentChangedInvoke()
    {
        if (OnHealthPercentChanged != null)
        {
            OnHealthPercentChanged.Invoke(CurrentHealth / MaxHealth);
        }
    }

    public void Damage(float damage)
    {        
        if (CurrentHealth <= 0)
        {
            return;
        }

        CurrentHealth -= damage*Time.deltaTime;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

        OnHealthPercentChangedInvoke();

        if (CurrentHealth <= 0)
        {
            Debug.LogError(gameObject + " zero health");
        }
    }
}
