using UnityEngine;

public class PlayerHealth : AbstractHealth
{
    [SerializeField] private FloatGameEvent OnHealthPercentChanged;

    private void Start()
    {
        OnHealthPercentChangedInvoke();
    }

    private void OnHealthPercentChangedInvoke()
    {
        OnHealthPercentChanged.Raise(CurrentHealth / Balance.MaxHealth);
    }

    protected override void CommonDamage(float damage)
    {
        base.CommonDamage(damage);
        OnHealthPercentChangedInvoke();
    }
}
