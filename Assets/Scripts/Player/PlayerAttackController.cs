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
        AddItem(6);
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
        return Mathf.Pow(Balance.Attacks[id].Balance.BaseDamage * level, Balance.Attacks[id].ValueLevelPow);
    }

    public bool IsChance(int id)
    {
        return Random.value < Balance.Attacks[id].Balance.Chance / 100f;
    }

    public float GetItemValueByCurrentLevel(int id)
    {
        float damage = GetItemValueByLevel(id, ItemLevelsByIds[id]);

        if (Balance.Attacks[id].Balance.CriticalMultiplier > 0 && IsChance(id))
        {
            damage *= Balance.Attacks[id].Balance.CriticalMultiplier;
        }

        return damage;
    }

    private IEnumerator AttackLoop(int id)
    {
        while (true)
        {
            yield return GetNextAttackDelay(id);

            StartCoroutine(CreateAttackAmount(id));
        }
    }

    private IEnumerator CreateAttackAmount(int id)
    {
        int amount = GetAmount(id);

        for (int i = 0; i < amount; i++)
        {
            BasePlayerAttack attackInstance = AttackPool.Instantiate(Balance.Attacks[id].Prefab.gameObject);
            attackInstance.Pool = AttackPool;
            attackInstance.Id = id;
            attackInstance.Balance = Balance.Attacks[id].Balance;
            attackInstance.StunWaitForSeconds = Balance.StunWaitForSecondsById[id];
            attackInstance.Init(
                new BasePlayerAttack.InitData(
                    PlayerMove.CurrentFaceDir, PlayerMove.GetLastMoveDir,
                    MobSpawnController.GetNearestMobPosition(PlayerTransform.position),
                    MobSpawnController.GetRandomMobPosition(PlayerTransform.position),
                    PlayerTransform, i, amount));

            if (i != Balance.Attacks[id].Balance.Amount - 1)
            {
                yield return attackInstance.ProjectileIntervalWaitForSeconds;
            }            
        }
    }

    private int GetAmount(int id)
    {
        int amount = Mathf.Clamp(Balance.Attacks[id].Balance.Amount, 1, Balance.Attacks[id].Balance.PoolLimit);
        return amount; // todo use upgrade amount and player stat amount
    }

    private WaitForSeconds GetNextAttackDelay(int id)
    {
        return new WaitForSeconds(Balance.Attacks[id].Balance.CooldownAfterDuration ?
            Balance.Attacks[id].Balance.Duration + Balance.Attacks[id].Balance.Cooldown
            : Balance.Attacks[id].Balance.Cooldown);
    }

    public void OnTriggerEnterAttack(PlayerAttackTrigger playerAttackTrigger)
    {
        float damage = GetItemValueByCurrentLevel(playerAttackTrigger.PlayerAttack.Id);
        MobHolder mobHolder = GetAttackedMobHolder(playerAttackTrigger);
        mobHolder.GetMobHealth.Damage(damage);
        mobHolder.GetRigidbody.AddForce(
                (mobHolder.GetTransform.position - PlayerTransform.position) *
                playerAttackTrigger.PlayerAttack.Balance.Knockback * 1000f);

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
