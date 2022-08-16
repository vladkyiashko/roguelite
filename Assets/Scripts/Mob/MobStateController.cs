using System.Collections;
using UnityEngine;

public class MobStateController : MonoBehaviour
{
    [SerializeField] private AbstractHealth Health;
    private Coroutine CurrentStunCor;
    public MobState State { get; private set; }

    private void Awake()
    {
        Health.OnZeroHealthAction += OnZeroHealth;
    }

    private void OnEnable()
    {
        State = MobState.none;
    }

    public void Stun(WaitForSeconds duration)
    {
        if (Health.GetCurrentHealth <= 0)
        {
            return;
        }

        if (CurrentStunCor != null)
        {
            StopCoroutine(CurrentStunCor);
        }

        CurrentStunCor = StartCoroutine(StunCor(duration));

    }

    public void OnAttack()
    {
        State = MobState.attacking;
    }

    public void OnAttackStop()
    {
        if (State == MobState.stunned)
        {
            return;
        }

        State = MobState.none;
    }

    public void OnMoved()
    {
        State = MobState.moving;
    }

    private void OnZeroHealth()
    {
        State = MobState.dying;
    }

    private IEnumerator StunCor(WaitForSeconds duration)
    {
        State = MobState.stunned;
        yield return duration;
        CurrentStunCor = null;
        State = MobState.none;
    }
}

public enum MobState
{
    none,
    moving,
    attacking,
    dying,
    stunned
}
