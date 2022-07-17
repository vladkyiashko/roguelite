using System;
using DataStructures.RandomSelector;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnvSpawnController : MonoBehaviour
{
    [SerializeField] private int ZoneSize;
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private GameObjectWeightInfo[] Zones;
    private DynamicRandomSelector<GameObjectWeightInfo> Selector;
    private Vector2 ZoneCenterOffset;
    private Dictionary<Vector2Int, EnvSpawnZone> ZoneInstanceByPositionIndex = new Dictionary<Vector2Int, EnvSpawnZone>();
    private readonly Vector2Int[] PositionIndexes = new Vector2Int[] 
    {
        new Vector2Int(0,0),
        new Vector2Int(0,1),
        new Vector2Int(0,2),
        new Vector2Int(1,0),
        new Vector2Int(1,1),
        new Vector2Int(1,2),
        new Vector2Int(2,0),
        new Vector2Int(2,1),
        new Vector2Int(2,2)
    };
    private Dictionary<Vector2Int, Vector2> PositionOffsetsByPositionIndexes;
    public Action<EnvSpawnZone> OnEnvSpawnZoneInstantiated;

    private void Awake()
    {
        ZoneCenterOffset = new Vector2(-ZoneSize/2f, ZoneSize/2f);

        PositionOffsetsByPositionIndexes = new Dictionary<Vector2Int, Vector2>
        {
            {PositionIndexes[0], new Vector2(-ZoneSize, ZoneSize) + ZoneCenterOffset},
            {PositionIndexes[1], new Vector2(-ZoneSize, 0) + ZoneCenterOffset},
            {PositionIndexes[2], new Vector2(-ZoneSize, -ZoneSize) + ZoneCenterOffset},
            {PositionIndexes[3], new Vector2(0, ZoneSize) + ZoneCenterOffset},
            {PositionIndexes[4], Vector2.zero + ZoneCenterOffset},
            {PositionIndexes[5], new Vector2(0, -ZoneSize) + ZoneCenterOffset},
            {PositionIndexes[6], new Vector2(ZoneSize, ZoneSize) + ZoneCenterOffset},
            {PositionIndexes[7], new Vector2(ZoneSize, 0) + ZoneCenterOffset},
            {PositionIndexes[8], new Vector2(ZoneSize, -ZoneSize) + ZoneCenterOffset}
        };

        Selector = new DynamicRandomSelector<GameObjectWeightInfo>();
        for (int i = 0; i < Zones.Length; i++)
        {
            Selector.Add(Zones[i], Zones[i].Weight);
        }
        Selector.Build();
    }

    private void Start()
    {
        Vector2 playerTransformPosition = PlayerTransform.position;
        foreach (Vector2Int positionIndex in PositionIndexes)
        {
            EnvSpawnZone zoneInstance = InstantiateZone(GetZonePosition(positionIndex, playerTransformPosition));
            zoneInstance.gameObject.name = positionIndex.ToString(); // TODO tmp?
            ZoneInstanceByPositionIndex.Add(positionIndex, zoneInstance);
        }
    }

    private Vector2 GetZonePosition(Vector2Int positionIndex, Vector2 centerPosition)
    {
        return centerPosition + PositionOffsetsByPositionIndexes[positionIndex];
    }

    private EnvSpawnZone InstantiateZone(Vector2 position)
    {
        GameObjectWeightInfo zone = Selector.SelectRandomItem();
        EnvSpawnZone envSpawnZoneInstance = LocalObjectPool.Instantiate(zone.Prefab).GetComponent<EnvSpawnZone>();
        envSpawnZoneInstance.GetTransform.position = position;
        envSpawnZoneInstance.OnTriggerPlayerEnter = OnPlayerEnter;
        
        if (OnEnvSpawnZoneInstantiated != null)
        {
           OnEnvSpawnZoneInstantiated.Invoke(envSpawnZoneInstance);
        }

        return envSpawnZoneInstance;
    }    

    private void OnPlayerEnter(EnvSpawnZone envSpawnZone)
    {
        Vector2Int positionIndex = ZoneInstanceByPositionIndex.FirstOrDefault(x => x.Value.GetTransform.position == envSpawnZone.GetTransform.position).Key;

        if (positionIndex == Vector2Int.one)
        {
            return;
        }

        Vector2Int positionIndexDiff = positionIndex - Vector2Int.one;        

        Dictionary<Vector2Int, EnvSpawnZone> newZoneInstanceByPositionIndex = new Dictionary<Vector2Int, EnvSpawnZone>();

        foreach (KeyValuePair<Vector2Int, EnvSpawnZone> zoneKeyValuePair in ZoneInstanceByPositionIndex)
        {
            if (Mathf.Abs(positionIndex.x - zoneKeyValuePair.Key.x) > 1 || Mathf.Abs(positionIndex.y - zoneKeyValuePair.Key.y) > 1)
            {
                LocalObjectPool.Destroy(zoneKeyValuePair.Value.gameObject);                
            }
            else
            {
                Vector2Int newPositionIndex = zoneKeyValuePair.Key - positionIndexDiff;
                newZoneInstanceByPositionIndex.Add(newPositionIndex, zoneKeyValuePair.Value);
            }
        }

        Vector2 positionCenter = (Vector2) newZoneInstanceByPositionIndex[Vector2Int.one].GetTransform.position - ZoneCenterOffset;      
        for (int i = 0; i < PositionIndexes.Length; i++)
        {
            if (!newZoneInstanceByPositionIndex.ContainsKey(PositionIndexes[i]))
            {
                EnvSpawnZone zoneInstance = InstantiateZone(GetZonePosition(PositionIndexes[i], positionCenter));                
                newZoneInstanceByPositionIndex.Add(PositionIndexes[i], zoneInstance);
            }
        }        

        ZoneInstanceByPositionIndex = new Dictionary<Vector2Int, EnvSpawnZone>(newZoneInstanceByPositionIndex);
    }
}
