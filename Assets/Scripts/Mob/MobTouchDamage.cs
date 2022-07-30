using UnityEngine;

public class MobTouchDamage : MonoBehaviour
{
    [SerializeField] private float Damage;
    [SerializeField] private MobMove MobMove;
    public PlayerHealth PlayerHealth { get; set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == Consts.PlayerTag)
        {
            MobMove.enabled = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {        
        if (other.gameObject.tag == Consts.PlayerTag)
        {
            PlayerHealth.DamageOverTime(Damage);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == Consts.PlayerTag)
        {
            MobMove.enabled = true;
        }
    }
}
