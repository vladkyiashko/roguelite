using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobSpawnController : MonoBehaviour
{
    [SerializeField] private MobSpawnBalance Balance;
    [SerializeField] private EnvSpawnController EnvSpawnController;
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private PlayerHealth PlayerHealth;
    [SerializeField] private float VisibleSqrLen = 50f;
    private LocalObjectPoolGeneric<MobHolder> Pool;
    private readonly List<Transform> MobTransforms = new();
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
        MobTransforms.Add(mobInstance.GetTransform);

        mobInstance.GetMobMove.Target = PlayerTransform;
        mobInstance.GetTransform.position = GetSpawnPosition;
    }

    public Vector3 GetNearestMobPosition(Vector3 comparePosition)
    {
        Vector3 result = default;
        float nearestSqrLen = float.MaxValue;

        for (int i = 0; i < MobTransforms.Count; i++)
        {
            float sqrLen = (MobTransforms[i].position - comparePosition).sqrMagnitude;
            if (sqrLen < nearestSqrLen)
            {
                nearestSqrLen = sqrLen;
                result = MobTransforms[i].position;
            }
        }

        return result;
    }

    public Vector3 GetRandomMobPosition(Vector3 comparePosition)
    {
        if (MobTransforms.Count == 0)
        {
            return Vector3.zero;
        }

        List<Transform> list = new();
        for (int i = 0; i < MobTransforms.Count; i++)
        {
            float sqrLen = (MobTransforms[i].position - comparePosition).sqrMagnitude;
            if (sqrLen < VisibleSqrLen)
            {
                list.Add(MobTransforms[i]);
            }
        }

        if (list.Count == 0)
        {
            list = MobTransforms;
        }

        return list[Random.Range(0, list.Count)].position;
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
