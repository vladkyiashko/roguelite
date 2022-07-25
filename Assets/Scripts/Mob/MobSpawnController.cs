using UnityEngine;
using DataStructures.RandomSelector;
using System.Collections.Generic;
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
    private List<MobHolder> MobInstances = new List<MobHolder>();
    private Coroutine SpawnLoopCoroutine;
    private Vector3 GetSpawnPosition => EnvSpawnController.GetRandomNotVisiblePosition();
    private Dictionary<GameObject, MobHolder> CachedMobHolderByGameObject = new Dictionary<GameObject, MobHolder>();

    private void Awake()
    {
        EnvSpawnController.OnEnvSpawnZoneInstantiated += OnEnvSpawnZoneInstantiated;
        InitSelector();
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
        WaitForSeconds delay = new WaitForSeconds(SpawnDelay);

        while (true)
        {
            yield return delay;
            Spawn();
        }
    }

    private void Spawn()
    {
        GameObjectWeightInfo mob = Selector.SelectRandomItem();

        GameObject mobInstance = LocalObjectPool.Instantiate(mob.Prefab);
        MobHolder mobHolder = null;
        if (!CachedMobHolderByGameObject.ContainsKey(mobInstance))        
        {
            mobHolder = mobInstance.GetComponent<MobHolder>();
            CachedMobHolderByGameObject.Add(mobInstance, mobHolder);
            mobHolder.GetMobTouchDamage.PlayerHealth = PlayerHealth;
        }

        mobHolder = CachedMobHolderByGameObject[mobInstance];
        mobHolder.GetMobMove.Target = PlayerTransform;
        mobHolder.GetTransform.position = GetSpawnPosition;
        MobInstances.Add(mobHolder);
    }

    private void OnEnvSpawnZoneInstantiated(EnvSpawnZone envSpawnZone)
    {
        envSpawnZone.OnTriggerMobExit = OnMobExitZone;
    }

    private void OnMobExitZone(EnvSpawnZone envSpawnZone, GameObject mob)
    {
        if (!envSpawnZone.gameObject.activeInHierarchy)
        {
            CachedMobHolderByGameObject[mob].GetTransform.position = GetSpawnPosition;
        }
    }
}
