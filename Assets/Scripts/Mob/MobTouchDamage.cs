using System;
using UnityEngine;
using UnityEngine.Events;

public class MobTouchDamage : MonoBehaviour
{
    [SerializeField] private MobBalance Balance;
    [SerializeField] private DamageEvents Events;
    [SerializeField] private FloatGameEvent OnDOTToPlayer;
    [SerializeField] private MobStateController StateController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == Consts.PlayerTag && StateController.State != MobState.dying)
        {
            StateController.OnAttack();
            Events.OnAttack.Invoke();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == Consts.PlayerTag && StateController.State != MobState.dying)
        {
            OnDOTToPlayer.Raise(Balance.Damage);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == Consts.PlayerTag && StateController.State != MobState.dying)
        {
            StateController.OnAttackStop();
        }
    }

    [Serializable]
    public struct DamageEvents
    {
        public UnityEvent OnAttack;
    }
}
