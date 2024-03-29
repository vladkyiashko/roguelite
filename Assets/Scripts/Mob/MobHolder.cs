using UnityEngine;

public class MobHolder : MonoBehaviour
{
    [SerializeField] private MobBalance Balance;
    [SerializeField] private MobMove MobMove;
    [SerializeField] private Transform Transform;
    [SerializeField] private MobTouchDamage MobTouchDamage;
    [SerializeField] private MobHealth MobHealth;
    [SerializeField] private Rigidbody2D Rigidbody;
    [SerializeField] private MobStateController StateController;
    public MobMove GetMobMove => MobMove;
    public Transform GetTransform => Transform;
    public MobTouchDamage GetMobTouchDamage => MobTouchDamage;
    public MobHealth GetMobHealth => MobHealth;
    public Rigidbody2D GetRigidbody => Rigidbody;
    public MobStateController GetMobStateController => StateController;
    public MobBalance GetBalance => Balance;
}

