using DataStructures.RandomSelector;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Balance/EnvSpawnBalance")]
public class EnvSpawnBalance : ScriptableObject
{
    public Vector2Int ZoneSizeDimensions;
    public GameObjectWeightInfo[] Zones;
    public Vector2Int ActiveZonesCountDimensions = new(5, 5);
    private DynamicRandomSelector<GameObjectWeightInfo> Selector;
    public DynamicRandomSelector<GameObjectWeightInfo> GetSelector
    {
        get
        {
            if (Selector == null)
            {
                InitSelector();
            }

            return Selector;
        }
    }

    private void InitSelector()
    {
        Selector = new DynamicRandomSelector<GameObjectWeightInfo>();
        for (int i = 0; i < Zones.Length; i++)
        {
            Selector.Add(Zones[i], Zones[i].Weight);
        }
        _ = Selector.Build();
    }
}
