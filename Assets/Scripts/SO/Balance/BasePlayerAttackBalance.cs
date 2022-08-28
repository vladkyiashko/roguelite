using UnityEngine;

[CreateAssetMenu(menuName = "SO/Balance/BasePlayerAttackBalance")]
public class BasePlayerAttackBalance : ScriptableObject
{
    public float Delay;
    public float Damage;
    public float PushForce;
    public float StunDuration;
    private WaitForSeconds _StunWaitForSeconds;
    public WaitForSeconds StunWaitForSeconds
    {
        get
        {
            _StunWaitForSeconds ??= new(StunDuration);

            return _StunWaitForSeconds;
        }
    }
}
