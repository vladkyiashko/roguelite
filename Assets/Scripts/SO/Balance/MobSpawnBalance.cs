using DataStructures.RandomSelector;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Balance/MobSpawnBalance")]
public class MobSpawnBalance : ScriptableObject
{
    public int InitialSpawnCount;
    public float SpawnDelay;
    public GameObjectWeightInfo[] Mobs;
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
        for (int i = 0; i < Mobs.Length; i++)
        {
            Selector.Add(Mobs[i], Mobs[i].Weight);
        }
        _ = Selector.Build();
    }
}
