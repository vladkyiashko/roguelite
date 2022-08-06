using System;
using UnityEngine;
using UnityEngine.Events;

public class MobTouchDamage : MonoBehaviour
{
    [SerializeField] private float Damage;
    [SerializeField] private DamageEvents Events;
    [SerializeField] private MobStateController StateController;
    public PlayerHealth PlayerHealth { get; set; }

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
            PlayerHealth.DamageOverTime(Damage);
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
