using UnityEngine;

public abstract class AbstractMove : MonoBehaviour
{
    [SerializeField] protected float Speed;
    [SerializeField] protected Rigidbody2D RB;
    protected Vector3 MoveDir;
    
    protected virtual void FixedUpdate()
    {
        if (MoveDir == Vector3.zero)
        {
            return;
        }

        RB.MovePosition(transform.position + MoveDir * Speed * Time.fixedDeltaTime);
    }
}
