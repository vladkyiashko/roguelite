using UnityEngine;

public class MobMove : AbstractMove
{
    [SerializeField] private Transform Transform;
    [SerializeField] private MobStateController MobStateController;
    public Transform GetTransform => Transform;
    public Transform Target { get; set; }

    protected override void FixedUpdate()
    {
        MoveDir = Target == null || MobStateController.State == MobState.stunned
            || MobStateController.State == MobState.attacking ?
            Vector3.zero : (Target.position - transform.position).normalized;

        if (MoveDir != Vector3.zero)
        {
            MobStateController.OnMoved();
        }

        base.FixedUpdate();
    }
}
