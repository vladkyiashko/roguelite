using System;

public class PlayerHealth : AbstractHealth
{
    public event Action<float> OnHealthPercentChanged;

    private void Start()
    {
        OnHealthPercentChangedInvoke();
    }

    private void OnHealthPercentChangedInvoke()
    {
        OnHealthPercentChanged?.Invoke(CurrentHealth / Balance.MaxHealth);
    }

    public override void DamageOverTime(float damage)
    {
        base.DamageOverTime(damage);
        OnHealthPercentChangedInvoke();
    }
}
