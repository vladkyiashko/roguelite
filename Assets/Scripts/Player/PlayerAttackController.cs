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
        WaitForSeconds delay = new(attack.GetBalance.Delay);

        while (true)
        {
            yield return delay;

            BasePlayerAttack attackInstance = AttackPool.Instantiate(attack.gameObject);

            attackInstance.GetTransform.position = PlayerTransform.position;

            attackInstance.GetTransform.localScale =
                PlayerMove.CurrentFaceDir == AbstractMove.FaceDir.Right ? Vector3.one : LeftScale;

            _ = StartCoroutine(DestroyDelayed(AttackPool, AttackDestroyDelay, attackInstance.GetTransform));
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
        mobHolder.GetMobHealth.Damage(playerAttackTrigger.PlayerAttack.GetBalance.Damage);
        mobHolder.GetRigidbody.AddForce(
                (mobHolder.GetTransform.position - PlayerTransform.position) *
                playerAttackTrigger.PlayerAttack.GetBalance.PushForce);

        if (playerAttackTrigger.PlayerAttack.GetBalance.StunWaitForSeconds != null)
        {
            mobHolder.GetMobStateController.Stun(playerAttackTrigger.PlayerAttack.GetBalance.StunWaitForSeconds);
        }

        DamageTextHolder damageTextInstance = DamageTextPool.Instantiate(DamageTextPrefab);
        damageTextInstance.GetTransform.SetParent(WorldSpaceCanvasTransform, true);
        damageTextInstance.GetTransform.position = mobHolder.GetTransform.position;
        damageTextInstance.GetText.text = playerAttackTrigger.PlayerAttack.GetBalance.Damage.ToString();

        _ = StartCoroutine(DestroyDelayed(DamageTextPool, DamageTextDestroyDelay, damageTextInstance.GetTransform));
    }

    private IEnumerator DestroyDelayed(ILocalObjectPoolGeneric pool, WaitForSeconds delay, Transform goTransform)
    {
        yield return delay;
        pool.Destroy(goTransform);
    }
}
