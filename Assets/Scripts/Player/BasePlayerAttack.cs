using UnityEngine;

public class BasePlayerAttack : MonoBehaviour
{
    [SerializeField] private PlayerAttackTriggerGameEvent OnTriggerEnter;
    [SerializeField] private Transform Transform;
    public Transform GetTransform => Transform;
    public int Id { get; set; }
    public BasePlayerAttackBalance Balance { get; set; }
    public WaitForSeconds StunWaitForSeconds { get; set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerEnter.Raise(new PlayerAttackTrigger(this, other.gameObject));
    }
}
