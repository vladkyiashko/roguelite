using DataStructures.RandomSelector;
using UnityEngine;

public class DropController : MonoBehaviour
{
    [SerializeField] private MobBalance Balance;
    [SerializeField] private AbstractHealth Health;
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
        lootInstance.Value = Balance.Exp;
        lootInstance.DropPickup = DropPickup;
        lootInstance.GetTransform.position = transform.position;
        lootInstance.OnPickup += OnPickup;
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

    private void OnPickup(Loot lootInstance)
    {
        lootInstance.OnPickup -= OnPickup;
        Pool.Destroy(lootInstance.GetTransform);
    }
}
