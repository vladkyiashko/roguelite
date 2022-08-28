using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private PlayerAttacksBalance Balance;
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private AbstractMove PlayerMove;
    [SerializeField] private GameObject DamageTextPrefab;
    [SerializeField] private Transform WorldSpaceCanvasTransform;
    private Vector3 LeftScale = new(-1, 1, 1);
    private List<BasePlayerAttack> ActiveAttackPrefabs = new();
    private Dictionary<BasePlayerAttack, Coroutine> AttackLoopsByAttack = new();
    private LocalObjectPoolGeneric<BasePlayerAttack> AttackPool;
    private Dictionary<GameObject, MobHolder> CachedMobHolderByGameObject = new();
    private LocalObjectPoolGeneric<DamageTextHolder> DamageTextPool;

    private void Awake()
    {
        AttackPool = new LocalObjectPoolGeneric<BasePlayerAttack>();
        DamageTextPool = new LocalObjectPoolGeneric<DamageTextHolder>();
    }

    private void Start()
    {
        ActiveAttackPrefabs.Add(Balance.Attacks[0].Prefab);

        for (int i = 0; i < ActiveAttackPrefabs.Count; i++)
        {
            AttackLoopsByAttack.Add(ActiveAttackPrefabs[i], StartCoroutine(AttackLoop(ActiveAttackPrefabs[i])));
        }
    }

    private IEnumerator AttackLoop(BasePlayerAttack attackPrefab)
    {
        while (true)
        {
            yield return Balance.DelayWaitForSecondsByAttackPrefab[attackPrefab];

            BasePlayerAttack attackInstance = AttackPool.Instantiate(attackPrefab.gameObject);
            attackInstance.Balance = Balance.AttackBalanceByAttackPrefab[attackPrefab];
            attackInstance.StunWaitForSeconds = Balance.StunWaitForSecondsByAttackPrefab[attackPrefab];

            attackInstance.GetTransform.position = PlayerTransform.position;

            attackInstance.GetTransform.localScale =
                PlayerMove.CurrentFaceDir == AbstractMove.FaceDir.Right ? Vector3.one : LeftScale;

            _ = StartCoroutine(DestroyDelayed(AttackPool, Balance.AttackDestroyDelayWaitForSeconds,
                        attackInstance.GetTransform));
        }
    }

    public void OnTriggerEnterAttack(PlayerAttackTrigger playerAttackTrigger)
    {
        if (!CachedMobHolderByGameObject.ContainsKey(playerAttackTrigger.OtherGameObject))
        {
            CachedMobHolderByGameObject.Add(playerAttackTrigger.OtherGameObject,
                    playerAttackTrigger.OtherGameObject.GetComponent<MobHolder>());
        }
        MobHolder mobHolder = CachedMobHolderByGameObject[playerAttackTrigger.OtherGameObject];
        mobHolder.GetMobHealth.Damage(playerAttackTrigger.PlayerAttack.Balance.Damage);
        mobHolder.GetRigidbody.AddForce(
                (mobHolder.GetTransform.position - PlayerTransform.position) *
                playerAttackTrigger.PlayerAttack.Balance.PushForce);

        mobHolder.GetMobStateController.Stun(playerAttackTrigger.PlayerAttack.StunWaitForSeconds);

        DamageTextHolder damageTextInstance = DamageTextPool.Instantiate(DamageTextPrefab);
        damageTextInstance.GetTransform.SetParent(WorldSpaceCanvasTransform, true);
        damageTextInstance.GetTransform.position = mobHolder.GetTransform.position;
        damageTextInstance.GetText.text = playerAttackTrigger.PlayerAttack.Balance.Damage.ToString();

        _ = StartCoroutine(DestroyDelayed(DamageTextPool, Balance.DamageTextDestroyDelayWaitForSeconds,
                    damageTextInstance.GetTransform));
    }

    private IEnumerator DestroyDelayed(ILocalObjectPoolGeneric pool, WaitForSeconds delay, Transform goTransform)
    {
        yield return delay;
        pool.Destroy(goTransform);
    }
}
