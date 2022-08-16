using UnityEngine;
using DataStructures.RandomSelector;
using System.Collections;

public class MobSpawnController : MonoBehaviour
{
    [SerializeField] private int InitialSpawnCount;
    [SerializeField] private float SpawnDelay;
    [SerializeField] private GameObjectWeightInfo[] Mobs;
    [SerializeField] private EnvSpawnController EnvSpawnController;
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private PlayerHealth PlayerHealth;
    private DynamicRandomSelector<GameObjectWeightInfo> Selector;
    private LocalObjectPoolGeneric<MobHolder> Pool;
    private Coroutine SpawnLoopCoroutine;
    private Vector3 GetSpawnPosition => EnvSpawnController.GetRandomNotVisiblePosition();

    private void Awake()
    {
        EnvSpawnController.OnEnvSpawnZoneInstantiated += OnEnvSpawnZoneInstantiated;
        InitSelector();
        Pool = new LocalObjectPoolGeneric<MobHolder>();
    }

    private void InitSelector()
    {
        Selector = new DynamicRandomSelector<GameObjectWeightInfo>();
        for (int i = 0; i < Mobs.Length; i++)
        {
            Selector.Add(Mobs[i], Mobs[i].Weight);
        }
        Selector.Build();
    }

    private void OnDestroy()
    {
        if (EnvSpawnController != null)            
        {
            EnvSpawnController.OnEnvSpawnZoneInstantiated -= OnEnvSpawnZoneInstantiated;
        }        
    }

    private void Start()
    {
        for (int i = 0; i < InitialSpawnCount; i++)
        {
            Spawn();
        }

        SpawnLoopCoroutine = StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        WaitForSeconds delay = new(SpawnDelay);

        while (true)
        {
            yield return delay;
            Spawn();
        }
    }

    private void Spawn()
    {
        GameObjectWeightInfo mob = Selector.SelectRandomItem();

        MobHolder mobInstance = Pool.Instantiate(mob.Prefab);
        if (!mobInstance.Inited)
        {
            mobInstance.Inited = true;
            mobInstance.GetMobTouchDamage.PlayerHealth = PlayerHealth;
            mobInstance.GetMobMove.Target = PlayerTransform;
            mobInstance.GetMobHealth.OnDeathAnimComplete += OnMobDeathAnimComplete;
        }
        mobInstance.GetTransform.position = GetSpawnPosition;
    }

    private void OnMobDeathAnimComplete(Transform mobTransform)
    {
        Pool.Destroy(mobTransform);
    }

    private void OnEnvSpawnZoneInstantiated(EnvSpawnZone envSpawnZone)
    {
        envSpawnZone.OnTriggerMobExit = OnMobExitZone;
    }

    private void OnMobExitZone(EnvSpawnZone envSpawnZone, GameObject mob)
    {
        if (!envSpawnZone.gameObject.activeInHierarchy)
        {
            mob.transform.position = GetSpawnPosition;
        }
    }
}
