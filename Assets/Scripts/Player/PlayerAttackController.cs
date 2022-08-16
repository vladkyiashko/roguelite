using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private BasePlayerAttack[] AttackPrefabs;
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private AbstractMove PlayerMove;
    [SerializeField] private GameObject DamageTextPrefab;
    [SerializeField] private Transform WorldSpaceCanvasTransform;
    private Vector3 LeftScale = new(-1, 1, 1);
    private List<BasePlayerAttack> ActiveAttacks = new();
    private Dictionary<BasePlayerAttack, Coroutine> AttackLoopsByAttack = new();
    private LocalObjectPoolGeneric<BasePlayerAttack> AttackPool;
    private Dictionary<GameObject, MobHolder> CachedMobHolderByGameObject = new();
    private LocalObjectPoolGeneric<DamageTextHolder> DamageTextPool;
    private readonly WaitForSeconds AttackDestroyDelay = new(0.5f);
    private readonly WaitForSeconds DamageTextDestroyDelay = new(1f);

    private void Awake()
    {
        AttackPool = new LocalObjectPoolGeneric<BasePlayerAttack>();
        DamageTextPool = new LocalObjectPoolGeneric<DamageTextHolder>();
    }

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

            BasePlayerAttack attackInstance = AttackPool.Instantiate(attack.gameObject);
            if (!attackInstance.Inited)
            {
                attackInstance.Inited = true;
                attackInstance.OnTriggerEnter += OnTriggerEnterAttack;
            }
            attackInstance.GetTransform.position = PlayerTransform.position;

            attackInstance.GetTransform.localScale =
                PlayerMove.CurrentFaceDir == AbstractMove.FaceDir.Right ? Vector3.one : LeftScale;

            StartCoroutine(DestroyDelayed(AttackPool, AttackDestroyDelay, attackInstance.GetTransform));
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
            mobHolder.GetMobStateController.Stun(attack.StunWaitForSeconds);
        }

        DamageTextHolder damageTextInstance = DamageTextPool.Instantiate(DamageTextPrefab);
        damageTextInstance.GetTransform.SetParent(WorldSpaceCanvasTransform, true);
        damageTextInstance.GetTransform.position = mobHolder.GetTransform.position;
        damageTextInstance.GetText.text = attack.GetDamage.ToString();

        StartCoroutine(DestroyDelayed(DamageTextPool, DamageTextDestroyDelay, damageTextInstance.GetTransform));
    }

    private IEnumerator DestroyDelayed(ILocalObjectPoolGeneric pool, WaitForSeconds delay, Transform goTransform)
    {
        yield return delay;
        pool.Destroy(goTransform);
    }
}
