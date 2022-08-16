using System;
using DataStructures.RandomSelector;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnvSpawnController : MonoBehaviour
{
    [SerializeField] private Vector2Int ZoneSizeDimensions;
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private GameObjectWeightInfo[] Zones;
    private DynamicRandomSelector<GameObjectWeightInfo> Selector;
    private Vector2 ZoneCenterOffset;
    private Dictionary<Vector2Int, EnvSpawnZone> ZoneInstanceByPositionIndex = new();
    private Vector2Int ActiveZonesCountDimensions = new(5, 5);
    private Vector2Int[] PositionIndexes;
    private Dictionary<Vector2Int, Vector2> PositionOffsetsByPositionIndexes;
    private Vector2Int MiddlePositionIndex;
    public event Action<EnvSpawnZone> OnEnvSpawnZoneInstantiated;    
    private List<Vector2Int> NotVisiblePositionIndexes;
    private LocalObjectPoolGeneric<EnvSpawnZone> Pool;

    private void Awake()
    {        
        InitPositionIndexes();
        ZoneCenterOffset = new Vector2(-ZoneSizeDimensions.x / 2f, ZoneSizeDimensions.y / 2f);
        InitPositionOffsetsByPositionIndexes();        
        InitSelector();
        MiddlePositionIndex = ActiveZonesCountDimensions / 2;
        InitNotVisiblePositionIndexes();
        Pool = new LocalObjectPoolGeneric<EnvSpawnZone>();
    }

    private void InitPositionIndexes()
    {
        PositionIndexes = new Vector2Int[ActiveZonesCountDimensions.x * ActiveZonesCountDimensions.y];
        int index = 0;
        for (int i = 0; i < ActiveZonesCountDimensions.x; i++)
        {
            for (int j = 0; j < ActiveZonesCountDimensions.y; j++)
            {
                PositionIndexes[index] = new Vector2Int(i, j);
                index++;
            }
        }
    }

    private void InitPositionOffsetsByPositionIndexes()
    {
        PositionOffsetsByPositionIndexes = new Dictionary<Vector2Int, Vector2>();
        int positionIndexesIndex = 0;
        int x = -ActiveZonesCountDimensions.x / 2 * ZoneSizeDimensions.x;		
		for (int i = 0; i < ActiveZonesCountDimensions.x; i++)
		{
			int y = ActiveZonesCountDimensions.y / 2 * ZoneSizeDimensions.y;
			for (int j = 0; j < ActiveZonesCountDimensions.y; j++)
			{
                PositionOffsetsByPositionIndexes.Add(PositionIndexes[positionIndexesIndex], new Vector2(x, y) + ZoneCenterOffset);
				y -= ZoneSizeDimensions.y;
                positionIndexesIndex++;
			}	
			x += ZoneSizeDimensions.x;
		}
    }    

    private void InitNotVisiblePositionIndexes()
    {
        NotVisiblePositionIndexes = new List<Vector2Int>();
        for(int i = 0; i < ActiveZonesCountDimensions.x; i++)
		{
			for(int j = 0; j < ActiveZonesCountDimensions.y; j++)
			{
				if ((Math.Abs(j - MiddlePositionIndex.y) >= MiddlePositionIndex.y) || (Math.Abs(i - MiddlePositionIndex.x) >= MiddlePositionIndex.x))
                {
                    NotVisiblePositionIndexes.Add(new Vector2Int(i, j));
                }
			}
		}
    }

    private void InitSelector()
    {
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

        EnvSpawnZone zoneInstance = Pool.Instantiate(zone.Prefab);
        if (!zoneInstance.Inited)
        {
            zoneInstance.Inited = true;
            zoneInstance.OnTriggerPlayerEnter += OnPlayerEnter;
        }
        zoneInstance.GetTransform.position = position;
        OnEnvSpawnZoneInstantiated?.Invoke(zoneInstance);

        return zoneInstance;
    }

    private void OnPlayerEnter(EnvSpawnZone envSpawnZone)
    {
        Vector2Int positionIndex = ZoneInstanceByPositionIndex.FirstOrDefault(x => x.Value.GetTransform.position == envSpawnZone.GetTransform.position).Key;

        if (positionIndex == MiddlePositionIndex)
        {
            return;
        }

        Vector2Int positionIndexDiff = positionIndex - MiddlePositionIndex;

        Dictionary<Vector2Int, EnvSpawnZone> newZoneInstanceByPositionIndex = new();
        List<EnvSpawnZone> destroyZoneQueue = new();

        foreach (KeyValuePair<Vector2Int, EnvSpawnZone> zoneKeyValuePair in ZoneInstanceByPositionIndex)
        {
            if (Mathf.Abs(positionIndex.x - zoneKeyValuePair.Key.x) > 2 || Mathf.Abs(positionIndex.y - zoneKeyValuePair.Key.y) > 2)
            {
                destroyZoneQueue.Add(zoneKeyValuePair.Value);
            }
            else
            {
                Vector2Int newPositionIndex = zoneKeyValuePair.Key - positionIndexDiff;
                newZoneInstanceByPositionIndex.Add(newPositionIndex, zoneKeyValuePair.Value);
            }
        }

        Vector2 positionCenter = (Vector2) newZoneInstanceByPositionIndex[MiddlePositionIndex].GetTransform.position - ZoneCenterOffset;
        for (int i = 0; i < PositionIndexes.Length; i++)
        {
            if (!newZoneInstanceByPositionIndex.ContainsKey(PositionIndexes[i]))
            {
                EnvSpawnZone zoneInstance = InstantiateZone(GetZonePosition(PositionIndexes[i], positionCenter));                
                newZoneInstanceByPositionIndex.Add(PositionIndexes[i], zoneInstance);
            }
        }        

        ZoneInstanceByPositionIndex = new Dictionary<Vector2Int, EnvSpawnZone>(newZoneInstanceByPositionIndex);

        for (int i = 0; i < destroyZoneQueue.Count; i++)
        {
            Pool.Destroy(destroyZoneQueue[i].GetTransform);
        }
    }

    public Vector3 GetRandomNotVisiblePosition()
    {        
        Vector2Int positionIndex = NotVisiblePositionIndexes[UnityEngine.Random.Range(0, NotVisiblePositionIndexes.Count)];
        Transform zoneTransform = ZoneInstanceByPositionIndex[positionIndex].GetTransform;
        
        Vector3 offset = new(UnityEngine.Random.Range(1, ZoneSizeDimensions.x), -UnityEngine.Random.Range(1, ZoneSizeDimensions.y), 0);

        return zoneTransform.position + offset;
    }
}
