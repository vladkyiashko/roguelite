using System;
using UnityEngine;
using UnityEngine.Events;

public class MobTouchDamage : MonoBehaviour
{
    [SerializeField] private float Damage;
    [SerializeField] private MobMove MobMove;
    [SerializeField] private DamageEvents Events;
    [SerializeField] private AbstractHealth Health;
    public PlayerHealth PlayerHealth { get; set; }

    private void OnEnable()
    {
        if (!MobMove.enabled)
        {
            MobMove.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == Consts.PlayerTag && Health.GetCurrentHealth > 0)
        {
            MobMove.enabled = false;
            MobMove.IsMoving = false;
            Events.OnAttack.Invoke();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == Consts.PlayerTag && Health.GetCurrentHealth > 0)
        {
            PlayerHealth.DamageOverTime(Damage);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == Consts.PlayerTag && Health.GetCurrentHealth > 0)
        {
            MobMove.enabled = true;
        }
    }
    [Serializable]
    public struct DamageEvents
    {
        public UnityEvent OnAttack;
    }
}
