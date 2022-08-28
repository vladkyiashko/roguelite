using UnityEngine;
using System.Collections;

public class MobSpawnController : MonoBehaviour
{
    [SerializeField] private MobSpawnBalance Balance;
    [SerializeField] private EnvSpawnController EnvSpawnController;
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private PlayerHealth PlayerHealth;
    private LocalObjectPoolGeneric<MobHolder> Pool;
    private Vector3 GetSpawnPosition => EnvSpawnController.GetRandomNotVisiblePosition();

    private void Awake()
    {
        Pool = new LocalObjectPoolGeneric<MobHolder>();
    }

    private void Start()
    {
        for (int i = 0; i < Balance.InitialSpawnCount; i++)
        {
            Spawn();
        }

        _ = StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        WaitForSeconds delay = new(Balance.SpawnDelay);

        while (true)
        {
            yield return delay;
            Spawn();
        }
    }

    private void Spawn()
    {
        GameObjectWeightInfo mob = Balance.GetSelector.SelectRandomItem();

        MobHolder mobInstance = Pool.Instantiate(mob.Prefab);

        mobInstance.GetMobMove.Target = PlayerTransform;
        mobInstance.GetTransform.position = GetSpawnPosition;
    }

    public void OnMobDeathAnimComplete(Transform mobTransform)
    {
        Pool.Destroy(mobTransform);
    }

    public void OnMobExitZone(EnvSpawnZoneTrigger envSpawnZoneTrigger)
    {
        if (!envSpawnZoneTrigger.EnvSpawnZoneGO.activeInHierarchy)
        {
            envSpawnZoneTrigger.OtherGameObject.transform.position = GetSpawnPosition;
        }
    }
}
