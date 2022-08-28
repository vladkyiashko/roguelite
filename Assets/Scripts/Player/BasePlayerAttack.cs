using UnityEngine;

public class BasePlayerAttack : MonoBehaviour
{
    [SerializeField] private PlayerAttackTriggerGameEvent OnTriggerEnter;
    [SerializeField] private BasePlayerAttackBalance Balance;
    [SerializeField] private Transform Transform;
    public Transform GetTransform => Transform;
    public BasePlayerAttackBalance GetBalance => Balance;

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerEnter.Raise(new PlayerAttackTrigger(this, other.gameObject));
    }
}
