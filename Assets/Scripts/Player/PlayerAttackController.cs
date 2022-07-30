using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private BasePlayerAttack[] AttackPrefabs;
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private AbstractMove PlayerMove;
    private Vector3 LeftScale = new(-1, 1, 1);
    private List<BasePlayerAttack> ActiveAttacks = new();
    private Dictionary<BasePlayerAttack, Coroutine> AttackLoopsByAttack = new();
    private Dictionary<GameObject, BasePlayerAttack> CachedAttackByGameObject = new();
    private Dictionary<GameObject, MobHolder> CachedMobHolderByGameObject = new();

    private void Start()
    {
        ActiveAttacks.Add(AttackPrefabs[0]);

        for (int i = 0; i < ActiveAttacks.Count; i++)
        {
            AttackLoopsByAttack.Add(ActiveAttacks[i], StartCoroutine(AttackLoop(ActiveAttacks[i])));
        }
    }

    private IEnumerator AttackLoop(BasePlayerAttack attack)
    {
        WaitForSeconds delay = new(attack.GetDelay);

        while (true)
        {
            yield return delay;

            GameObject attackInstance = LocalObjectPool.Instantiate(attack.gameObject);
            if (!CachedAttackByGameObject.ContainsKey(attackInstance))
            {
                CachedAttackByGameObject.Add(attackInstance, attackInstance.GetComponent<BasePlayerAttack>());
                CachedAttackByGameObject[attackInstance].OnTriggerEnter += OnTriggerEnterAttack;
            }
            CachedAttackByGameObject[attackInstance].GetTransform.position = PlayerTransform.position;

            CachedAttackByGameObject[attackInstance].GetTransform.localScale =
                PlayerMove.CurrentFaceDir == AbstractMove.FaceDir.Right ? Vector3.one : LeftScale;
        }
    }

    private void OnTriggerEnterAttack(BasePlayerAttack attack, Collider2D collider)
    {
        if (!CachedMobHolderByGameObject.ContainsKey(collider.gameObject))
        {
            CachedMobHolderByGameObject.Add(collider.gameObject, collider.GetComponent<MobHolder>());
        }
        MobHolder mobHolder = CachedMobHolderByGameObject[collider.gameObject];
        mobHolder.GetMobHealth.Damage(attack.GetDamage);
        mobHolder.GetRigidbody.AddForce(
                (mobHolder.GetTransform.position - PlayerTransform.position) * attack.GetPushForce);
        if (attack.StunWaitForSeconds != null)
        {
            mobHolder.GetMobMove.Stun(attack.StunWaitForSeconds);
        }
    }
}
