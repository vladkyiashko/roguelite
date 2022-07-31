using System.Collections;
using UnityEngine;

public class MobMove : AbstractMove
{
    [SerializeField] private Transform Transform;
    public Transform GetTransform => Transform;
    public Transform Target { get; set; }
    private Coroutine CurrentStunCor;
    private bool Stunned;

    public void Stun(WaitForSeconds duration)
    {
        if (CurrentStunCor != null)
        {
            StopCoroutine(CurrentStunCor);
        }

        CurrentStunCor = StartCoroutine(StunCor(duration));
    }

    private IEnumerator StunCor(WaitForSeconds duration)
    {
        Stunned = true;
        yield return duration;
        CurrentStunCor = null;
        Stunned = false;
    }

    protected override void FixedUpdate()
    {
        MoveDir = Target == null || Stunned ? Vector3.zero : (Target.position - transform.position).normalized;

        base.FixedUpdate();
    }
}
