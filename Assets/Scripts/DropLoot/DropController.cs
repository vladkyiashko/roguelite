using System.Collections.Generic;
using DataStructures.RandomSelector;
using UnityEngine;

public class DropController : MonoBehaviour
{
    [SerializeField] private MobBalance Balance;
    private DynamicRandomSelector<GameObjectWeightInfo> Selector;
    private LocalObjectPoolGeneric<Loot> Pool;
    private List<Loot> ActiveLootInstances = new();

    private void Awake()
    {
        InitSelector();
        Pool = new LocalObjectPoolGeneric<Loot>();
    }

    public void OnZeroHealth()
    {
        GameObjectWeightInfo dropInfo = Selector.SelectRandomItem();
        if (dropInfo.Prefab == null)
        {
            return;
        }

        Loot lootInstance = Pool.Instantiate(dropInfo.Prefab);
        ActiveLootInstances.Add(lootInstance);
        lootInstance.Value = Balance.Exp;
        lootInstance.GetTransform.position = transform.position;
    }

    private void InitSelector()
    {
        Selector = new DynamicRandomSelector<GameObjectWeightInfo>();
        for (int i = 0; i < Balance.Drops.Length; i++)
        {
            Selector.Add(Balance.Drops[i], Balance.Drops[i].Weight);
        }
        _ = Selector.Build();
    }

    public void OnPickup(Loot lootInstance)
    {
        Debug.Log(gameObject + " OnPickup " + lootInstance.gameObject + "; instances.Containes " + ActiveLootInstances.Contains(lootInstance));
        if (!ActiveLootInstances.Contains(lootInstance))
        {
            return;
        }

        Debug.LogError("pool.destroy " + lootInstance.gameObject);
        _ = ActiveLootInstances.Remove(lootInstance);
        Pool.Destroy(lootInstance.GetTransform);
    }
}
