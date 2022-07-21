using UnityEngine;

public class MobMove : AbstractMove
{
    [SerializeField] private Transform Transform;
    public Transform GetTransform => Transform;        
    public Transform Target { get; set; }    
    
    protected override void FixedUpdate()
    {
        if (Target == null)
        {
            return;
        }

        MoveDir = (Target.position - transform.position).normalized;

        base.FixedUpdate();
    }
}
