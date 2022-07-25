using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float MaxHealth;
    public float CurrentHealth; // todo make [SerializeField] private

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public void Damage(float damage)
    {        
        if (CurrentHealth <= 0)
        {
            //Debug.LogError("ignore damage");
            return;
        }
        else
        {
            //Debug.LogError("do damage");
        }

        CurrentHealth -= damage*Time.deltaTime;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

        if (CurrentHealth <= 0)
        {
            Debug.LogError(gameObject + " zero health");
        }
    }
}
