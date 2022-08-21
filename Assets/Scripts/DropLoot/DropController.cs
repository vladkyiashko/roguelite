using DataStructures.RandomSelector;
using UnityEngine;

public class DropController : MonoBehaviour
{
    [SerializeField] private AbstractHealth Health;
    [SerializeField] private GameObjectWeightInfo[] Drops;
    private DynamicRandomSelector<GameObjectWeightInfo> Selector;
    private LocalObjectPoolGeneric<Loot> Pool;
    public DropPickup DropPickup { get; set; }

    private void Awake()
    {
        InitSelector();
        Health.OnZeroHealthAction += OnZeroHealth;
        Pool = new LocalObjectPoolGeneric<Loot>();
    }

    private void OnDestroy()
    {
        Health.OnZeroHealthAction -= OnZeroHealth;
    }

    private void OnZeroHealth()
    {
        GameObjectWeightInfo dropInfo = Selector.SelectRandomItem();
        if (dropInfo.Prefab == null)
        {
            return;
        }

        Loot lootInstance = Pool.Instantiate(dropInfo.Prefab);
        lootInstance.DropPickup = DropPickup;
        lootInstance.GetTransform.position = transform.position;
    }

    private void InitSelector()
    {
        Selector = new DynamicRandomSelector<GameObjectWeightInfo>();
        for (int i = 0; i < Drops.Length; i++)
        {
            Selector.Add(Drops[i], Drops[i].Weight);
        }
        _ = Selector.Build();
    }
}
