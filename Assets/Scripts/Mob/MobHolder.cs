using UnityEngine;

public class MobHolder : MonoBehaviour
{
   [SerializeField] private MobMove MobMove;
   [SerializeField] private Transform Transform;
   [SerializeField] private MobTouchDamage MobTouchDamage;
   public MobMove GetMobMove => MobMove;
   public Transform GetTransform => Transform;
   public MobTouchDamage GetMobTouchDamage => MobTouchDamage;
}

