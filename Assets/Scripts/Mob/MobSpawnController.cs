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
    private DynamicRandomSelector<GameObjectWeightInfo> Selector;
    private List<Transform> MobInstances = new List<Transform>();
    private Coroutine SpawnLoopCoroutine;
    private Vector3 GetSpawnPosition => EnvSpawnController.GetRandomNotVisiblePosition();

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
        Transform mobTransform = LocalObjectPool.Instantiate(mob.Prefab).transform;
        mobTransform.position = GetSpawnPosition;
        MobInstances.Add(mobTransform);
    }

    private void OnEnvSpawnZoneInstantiated(EnvSpawnZone envSpawnZone)
    {
        envSpawnZone.OnTriggerMobExit = OnMobExit;
    }

    private void OnMobExit(EnvSpawnZone envSpawnZone, GameObject mob)
    {
        if (!envSpawnZone.gameObject.activeInHierarchy)
        {
            Debug.LogError("despawn " + mob + " because " + envSpawnZone.gameObject + " is inactive");
            LocalObjectPool.Destroy(mob);
            MobInstances.Remove(mob.transform);
        }
    }
}
