using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private PlayerAttacksBalance Balance;
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private AbstractMove PlayerMove;
    [SerializeField] private MobSpawnController MobSpawnController;
    [SerializeField] private GameObject DamageTextPrefab;
    [SerializeField] private Transform WorldSpaceCanvasTransform;    
    private readonly List<int> ActiveAttackIds = new();
    private readonly Dictionary<int, int> ItemLevelsByIds = new();
    private readonly Dictionary<int, Coroutine> AttackLoopsById = new();
    private LocalObjectPoolGeneric<BasePlayerAttack> AttackPool;
    private readonly Dictionary<GameObject, MobHolder> CachedMobHolderByGameObject = new();
    private LocalObjectPoolGeneric<DamageTextHolder> DamageTextPool;
    public List<int> GetActiveAttackIds => ActiveAttackIds;
    public Dictionary<int, int> GetItemLevelsByIds => ItemLevelsByIds;

    private void Awake()
    {
        AttackPool = new LocalObjectPoolGeneric<BasePlayerAttack>();
        DamageTextPool = new LocalObjectPoolGeneric<DamageTextHolder>();
    }

    private void Start()
    {
        AddItem(3);
    }

    public void AddItem(int id)
    {
        if (ActiveAttackIds.Contains(id))
        {
            ItemLevelsByIds[id]++;
        }
        else
        {
            ActiveAttackIds.Add(id);
            ItemLevelsByIds.Add(id, 1);

            AttackLoopsById.Add(id, StartCoroutine(AttackLoop(id)));
        }          
    }

    public float GetItemValueByLevel(int id, int level)
    {
        return Mathf.Pow(Balance.Attacks[id].Balance.Damage * level, Balance.Attacks[id].ValueLevelPow);
    }

    public float GetItemValueByCurrentLevel(int id)
    {
        return GetItemValueByLevel(id, ItemLevelsByIds[id]);
    }

    private IEnumerator AttackLoop(int id)
    {
        while (true)
        {
            yield return Balance.DelayWaitForSecondsById[id];

            BasePlayerAttack attackInstance = AttackPool.Instantiate(Balance.Attacks[id].Prefab.gameObject);
            attackInstance.Pool = AttackPool;
            attackInstance.Id = id;
            attackInstance.Balance = Balance.Attacks[id].Balance;
            attackInstance.StunWaitForSeconds = Balance.StunWaitForSecondsById[id];
            attackInstance.Init(
                new BasePlayerAttack.InitData(
                    PlayerMove.CurrentFaceDir, PlayerMove.GetLastMoveDir,
                    MobSpawnController.GetNearestMobPosition(PlayerTransform.position),
                    MobSpawnController.GetRandomMobPosition(PlayerTransform.position), PlayerTransform.position));
        }
    }

    public void OnTriggerEnterAttack(PlayerAttackTrigger playerAttackTrigger)
    {
        float damage = GetItemValueByCurrentLevel(playerAttackTrigger.PlayerAttack.Id);
        MobHolder mobHolder = GetAttackedMobHolder(playerAttackTrigger);
        mobHolder.GetMobHealth.Damage(damage);
        mobHolder.GetRigidbody.AddForce(
                (mobHolder.GetTransform.position - PlayerTransform.position) *
                playerAttackTrigger.PlayerAttack.Balance.PushForce);

        mobHolder.GetMobStateController.Stun(playerAttackTrigger.PlayerAttack.StunWaitForSeconds);

        DamageTextHolder damageTextInstance = DamageTextPool.Instantiate(DamageTextPrefab);
        damageTextInstance.GetTransform.SetParent(WorldSpaceCanvasTransform, true);
        damageTextInstance.GetTransform.position = mobHolder.GetTransform.position;
        damageTextInstance.GetText.text = damage.ToString();

        _ = StartCoroutine(DestroyDelayed(DamageTextPool, Balance.DamageTextDestroyDelayWaitForSeconds,
                    damageTextInstance.GetTransform));
    }

    private MobHolder GetAttackedMobHolder(PlayerAttackTrigger playerAttackTrigger)
    {
        if (!CachedMobHolderByGameObject.ContainsKey(playerAttackTrigger.OtherGameObject))
        {
            CachedMobHolderByGameObject.Add(playerAttackTrigger.OtherGameObject,
                    playerAttackTrigger.OtherGameObject.GetComponent<MobHolder>());
        }
        MobHolder mobHolder = CachedMobHolderByGameObject[playerAttackTrigger.OtherGameObject];
        return mobHolder;
    }

    private IEnumerator DestroyDelayed(ILocalObjectPoolGeneric pool, WaitForSeconds delay, Transform goTransform)
    {
        yield return delay;
        pool.Destroy(goTransform);
    }
}
